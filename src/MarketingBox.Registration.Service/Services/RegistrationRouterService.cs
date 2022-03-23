using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationRouterService : IRegistrationRouterService
    {
        private readonly IMyNoSqlServerDataReader<CampaignRowNoSql> _campaignRowNoSqlServerDataReader;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity> _dataReader;
        private readonly IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity> _dataWriter;
        private readonly IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity> _capacitorReader;
        private readonly IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity> _capacitorWriter;
        private readonly ILogger<RegistrationRouterService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private int GetIndexForNextBrand(
            string tenantId,
            long campaignId,
            IList<CampaignRowNoSql> campaigns)
        {
            var leadRouter = _dataReader.Get(RegistrationRouterNoSqlEntity.GeneratePartitionKey(tenantId),
                RegistrationRouterNoSqlEntity.GenerateRowKey(campaignId));
            int index;
            var lastRegisteredBrandId = leadRouter?.NoSqlInfo.LastRegisteredBrandId;
            // take the first campaign if cache is empty
            if (lastRegisteredBrandId is null)
            {
                index = 0;
            }
            else
            {
                // take index of last registered campaign or the first one in otherwise.
                index = campaigns.IndexOf(
                    campaigns.FirstOrDefault(
                        x =>
                            x.BrandId == lastRegisteredBrandId) ??
                    campaigns.First());
                // take the first campaign if previous registered campaign was the last in list.  
                if (++index == campaigns.Count)
                {
                    index = 0;
                }
            }

            return index;
        }

        public RegistrationRouterService(
            IMyNoSqlServerDataReader<CampaignRowNoSql> campaignRowNoSqlServerDataReader,
            IRegistrationRepository registrationRepository,
            IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity> dataReader,
            IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity> dataWriter,
            IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity> capacitorReader,
            IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity> capacitorWriter,
            ILogger<RegistrationRouterService> logger)
        {
            _campaignRowNoSqlServerDataReader = campaignRowNoSqlServerDataReader;
            _registrationRepository = registrationRepository;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            _capacitorReader = capacitorReader;
            _capacitorWriter = capacitorWriter;
            _logger = logger;
        }

        public async Task<List<CampaignRowNoSql>> GetSuitableRoutes(long campaignId, int countryId)
        {
            var date = DateTime.UtcNow;
            IReadOnlyList<CampaignRowNoSql> campaignRows =
                _campaignRowNoSqlServerDataReader.Get(CampaignRowNoSql.GeneratePartitionKey(campaignId));

            var filtered = new List<CampaignRowNoSql>(campaignRows.Count);

            foreach (var currentCampaignRow in campaignRows)
            {
                if (!currentCampaignRow.EnableTraffic)
                {
                    continue;
                }

                if (!(currentCampaignRow.Geo is null) &&
                    !currentCampaignRow.Geo.CountryIds.Contains(countryId))
                {
                    continue;
                }

                var activityHours = currentCampaignRow.ActivityHours.FirstOrDefault(x => x.Day == date.DayOfWeek);
                if (activityHours == null || !activityHours.IsActive)
                {
                    continue;
                }

                if (activityHours.From.HasValue && date.TimeOfDay < activityHours.From.Value)
                {
                    continue;
                }

                if (activityHours.To.HasValue && date.TimeOfDay > activityHours.To.Value)
                {
                    continue;
                }

                long currentCap = currentCampaignRow.CapType switch
                {
                    CapType.Lead => await _registrationRepository.GetCountForRegistrations(date,
                        currentCampaignRow.BrandId, currentCampaignRow.CampaignId, RegistrationStatus.Registered),
                    CapType.Ftds => await _registrationRepository.GetCountForDeposits(date, currentCampaignRow.BrandId,
                        currentCampaignRow.CampaignId, RegistrationStatus.Approved),
                    _ => 0
                };

                if (currentCampaignRow.DailyCapValue <= currentCap)
                {
                    continue;
                }

                filtered.Add(currentCampaignRow);
            }

            _logger.LogInformation("select From campaigns {@context}", new {CampaignsCount = filtered.Count});
            return filtered;
        }


        public async Task<CampaignRowNoSql> GetCampaignBox(
            string tenantId,
            long campaignId,
            List<CampaignRowNoSql> filtered)
        {
            await _semaphore.WaitAsync();

            try
            {
                var capacitors =
                    _capacitorReader.Get(RegistrationRouterCapacitorBoxNoSqlEntity.GeneratePartitionKey(campaignId));
                Dictionary<long, RegistrationRouterCapacitorBoxNoSqlEntity> countDict;

                bool saveAll = false;

                if (capacitors == null || !capacitors.Any())
                {
                    saveAll = true;
                    countDict = filtered.ToDictionary(x => x.CampaignRowId,
                        y => RegistrationRouterCapacitorBoxNoSqlEntity.Create(new RegistrationRouteCapacitorNoSqlInfo()
                        {
                            CampaignId = campaignId,
                            CampaignRowId = y.CampaignRowId,
                            ProcessedRegistration = 0
                        }));
                }
                else
                {
                    countDict = capacitors.ToDictionary(x => x.NoSqlInfo.CampaignRowId);
                }

                var priorities = filtered
                    .Select(x => x.Priority)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
                var ordered = filtered.ToLookup(x => x.Priority);

                //todo finish this routing algo
                foreach (var priority in priorities)
                {
                    do
                    {
                        //first loop;
                        var campaigns = ordered[priority]
                            .OrderByDescending(x => x.Weight)
                            .ToList();
                        do
                        {
                            var campaign = campaigns[GetIndexForNextBrand(tenantId, campaignId, campaigns)];

                            if (!countDict.TryGetValue(campaign.CampaignRowId, out var capacitor))
                            {
                                capacitor = RegistrationRouterCapacitorBoxNoSqlEntity.Create(
                                    new RegistrationRouteCapacitorNoSqlInfo()
                                    {
                                        CampaignId = campaignId,
                                        CampaignRowId = campaign.CampaignRowId,
                                        ProcessedRegistration = 0
                                    });
                                await _capacitorWriter.InsertOrReplaceAsync(capacitor);
                            }

                            if (capacitor.NoSqlInfo.ProcessedRegistration == campaign.Weight)
                            {
                                campaigns.Remove(campaign);
                                continue;
                            }

                            capacitor.NoSqlInfo.ProcessedRegistration++;

                            await _dataWriter.InsertOrReplaceAsync(RegistrationRouterNoSqlEntity.Create(
                                new RegistrationRouterNoSqlInfo()
                                {
                                    LastRegisteredBrandId = campaign.BrandId,
                                    CampaignId = campaignId,
                                    TenantId = tenantId
                                }));

                            if (!saveAll)
                            {
                                await _capacitorWriter.InsertOrReplaceAsync(capacitor);
                            }
                            else
                            {
                                foreach (var keyVal in countDict)
                                {
                                    await _capacitorWriter.InsertOrReplaceAsync(keyVal.Value);
                                }
                            }

                            return campaign;
                        } while (0 < campaigns.Count);

                        //Reset all
                        await _dataWriter.InsertOrReplaceAsync(RegistrationRouterNoSqlEntity.Create(
                            new RegistrationRouterNoSqlInfo()
                            {
                                LastRegisteredBrandId = null,
                                CampaignId = campaignId,
                                TenantId = tenantId
                            }));
                        foreach (var keyVal in countDict)
                        {
                            keyVal.Value.NoSqlInfo.ProcessedRegistration = 0;
                            await _capacitorWriter.InsertOrReplaceAsync(keyVal.Value);
                        }
                    } while (true);
                }

                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}