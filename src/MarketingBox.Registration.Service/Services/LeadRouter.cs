using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.MyNoSql.LeadRouter;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Services
{
    public class LeadRouter
    {
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly ILeadRepository _leadRepository;
        private readonly IMyNoSqlServerDataReader<LeadRouterNoSqlEntity> _dataReader;
        private readonly IMyNoSqlServerDataWriter<LeadRouterNoSqlEntity> _dataWriter;
        private readonly IMyNoSqlServerDataReader<LeadRouterCapacitorBoxNoSqlEntity> _capacitorReader;
        private readonly IMyNoSqlServerDataWriter<LeadRouterCapacitorBoxNoSqlEntity> _capacitorWriter;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public LeadRouter(
            IMyNoSqlServerDataReader<CampaignBoxNoSql> campaignBoxNoSqlServerDataReader,
            ILeadRepository leadRepository,
            IMyNoSqlServerDataReader<LeadRouterNoSqlEntity> dataReader,
            IMyNoSqlServerDataWriter<LeadRouterNoSqlEntity> dataWriter,
            IMyNoSqlServerDataReader<LeadRouterCapacitorBoxNoSqlEntity> capacitorReader,
            IMyNoSqlServerDataWriter<LeadRouterCapacitorBoxNoSqlEntity> capacitorWriter)
        {
            _campaignBoxNoSqlServerDataReader = campaignBoxNoSqlServerDataReader;
            _leadRepository = leadRepository;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            _capacitorReader = capacitorReader;
            _capacitorWriter = capacitorWriter;
        }

        public async Task<CampaignBoxNoSql> GetCampaignBox(string tenantId, long boxId, string country)
        {
            var date = DateTime.UtcNow;
            var campaignBoxes = _campaignBoxNoSqlServerDataReader.Get(CampaignBoxNoSql.GeneratePartitionKey(boxId));

            List<CampaignBoxNoSql> filtered = new List<CampaignBoxNoSql>(campaignBoxes.Count);

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
                    currentCap = await _leadRepository.GetCountForLeads(date,
                        currentCampaign.CampaignId, LeadStatus.Registered);
                }
                else if (currentCampaign.CapType == CapType.Ftds)
                {
                    currentCap = await _leadRepository.GetCountForDeposits(date,
                        currentCampaign.CampaignId, LeadStatus.Approved);
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

                var leadRouter = _dataReader.Get(LeadRouterNoSqlEntity.GeneratePartitionKey(tenantId),
                    LeadRouterNoSqlEntity.GenerateRowKey(boxId));

                var leadsRouted = leadRouter?.NoSqlInfo.LeadsRoutedCount ?? 0;

                var capacitors = _capacitorReader.Get(LeadRouterCapacitorBoxNoSqlEntity.GeneratePartitionKey(boxId));
                Dictionary<long, LeadRouterCapacitorBoxNoSqlEntity> countDict;

                bool saveAll = false;

                if (capacitors == null || !capacitors.Any())
                {
                    saveAll = true;
                    countDict = filtered.ToDictionary(x => x.CampaignBoxId, y => LeadRouterCapacitorBoxNoSqlEntity.Create(new LeadRouteCapacitorNoSqlInfo()
                    {
                        BoxId = boxId,
                        CampaignBoxId = y.CampaignBoxId,
                        ProcessedLeads = 0
                    }));
                }
                else
                {
                    countDict = capacitors.ToDictionary(x => x.NoSqlInfo.CampaignBoxId);
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
                            var capacitor = countDict[campaign.CampaignBoxId];
                            if (capacitor.NoSqlInfo.ProcessedLeads == campaign.Weight)
                            {
                                length--;
                                continue;
                            }

                            leadsRouted++;
                            capacitor.NoSqlInfo.ProcessedLeads++;

                            await _dataWriter.InsertOrReplaceAsync(LeadRouterNoSqlEntity.Create(
                                new LeadRouterNoSqlInfo()
                                {
                                    LeadsRoutedCount = leadsRouted,
                                    BoxId = boxId,
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
                            keyVal.Value.NoSqlInfo.ProcessedLeads = 0;
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