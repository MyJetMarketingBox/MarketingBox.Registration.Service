using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Registration.Service.Domain.Repositories;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationRouter
    {
        private readonly IMyNoSqlServerDataReader<CampaignRowNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity> _dataReader;
        private readonly IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity> _dataWriter;
        private readonly IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity> _capacitorReader;
        private readonly IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity> _capacitorWriter;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public RegistrationRouter(
            IMyNoSqlServerDataReader<CampaignRowNoSql> campaignBoxNoSqlServerDataReader,
            IRegistrationRepository registrationRepository,
            IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity> dataReader,
            IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity> dataWriter,
            IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity> capacitorReader,
            IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity> capacitorWriter)
        {
            _campaignBoxNoSqlServerDataReader = campaignBoxNoSqlServerDataReader;
            _registrationRepository = registrationRepository;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            _capacitorReader = capacitorReader;
            _capacitorWriter = capacitorWriter;
        }

        public async Task<CampaignRowNoSql> GetCampaignBox(string tenantId, long campaignId, string country)
        {
            var date = DateTime.UtcNow;
            var campaignBoxes = _campaignBoxNoSqlServerDataReader.Get(CampaignRowNoSql.GeneratePartitionKey(campaignId));

            var filtered = new List<CampaignRowNoSql>(campaignBoxes.Count);

            foreach (var currentCampaign in campaignBoxes)
            {
                if (!currentCampaign.EnableTraffic)
                {
                    continue;
                }

                if (!currentCampaign.CountryCode.Contains(country))
                {
                    continue;
                }

                long currentCap = 0;
                if (currentCampaign.CapType == CapType.Lead)
                {
                    currentCap = await _registrationRepository.GetCountForRegistrations(date,
                        currentCampaign.CampaignId, RegistrationStatus.Registered);
                }
                else if (currentCampaign.CapType == CapType.Ftds)
                {
                    currentCap = await _registrationRepository.GetCountForDeposits(date,
                        currentCampaign.CampaignId, RegistrationStatus.Approved);
                }

                if (currentCampaign.DailyCapValue <= currentCap)
                {
                    continue;
                }

                var activityHours = currentCampaign.ActivityHours.FirstOrDefault(x => x.Day == date.DayOfWeek);
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

                filtered.Add(currentCampaign);
            }

            if (!filtered.Any())
                return null; 

            await _semaphore.WaitAsync();

            try
            {

                var leadRouter = _dataReader.Get(RegistrationRouterNoSqlEntity.GeneratePartitionKey(tenantId),
                    RegistrationRouterNoSqlEntity.GenerateRowKey(campaignId));

                var leadsRouted = leadRouter?.NoSqlInfo.RegistrationsRoutedCount ?? 0;

                var capacitors = _capacitorReader.Get(RegistrationRouterCapacitorBoxNoSqlEntity.GeneratePartitionKey(campaignId));
                Dictionary<long, RegistrationRouterCapacitorBoxNoSqlEntity> countDict;

                bool saveAll = false;

                if (capacitors == null || !capacitors.Any())
                {
                    saveAll = true;
                    countDict = filtered.ToDictionary(x => x.CampaignRowId, y => RegistrationRouterCapacitorBoxNoSqlEntity.Create(new RegistrationRouteCapacitorNoSqlInfo()
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
                            .ToArray();

                        var length = campaigns.Length;

                        do
                        {
                            var index = leadsRouted % length;

                            var campaign = campaigns[index];
                            var capacitor = countDict[campaign.CampaignRowId];
                            if (capacitor.NoSqlInfo.ProcessedRegistration == campaign.Weight)
                            {
                                length--;
                                continue;
                            }

                            leadsRouted++;
                            capacitor.NoSqlInfo.ProcessedRegistration++;

                            await _dataWriter.InsertOrReplaceAsync(RegistrationRouterNoSqlEntity.Create(
                                new RegistrationRouterNoSqlInfo()
                                {
                                    RegistrationsRoutedCount = leadsRouted,
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
                        } while (0 < length);

                        //Reset all 
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