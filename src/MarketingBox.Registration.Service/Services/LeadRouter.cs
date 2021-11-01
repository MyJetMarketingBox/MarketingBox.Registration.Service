using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _tenantId;
        private readonly ILeadRepository _leadRepository;
        private readonly IMyNoSqlServerDataReader<LeadRouterNoSqlEntity> _dataReader;
        private readonly IMyNoSqlServerDataWriter<LeadRouterNoSqlEntity> _dataWriter;
        private readonly IMyNoSqlServerDataReader<LeadRouterCapacitorBoxNoSqlEntity> _capacitorReader;
        private readonly IMyNoSqlServerDataWriter<LeadRouterCapacitorBoxNoSqlEntity> _capacitorWriter;

        private readonly CampaignBoxNoSql[] _campaignBoxes;
        public long BoxId { get; }

        public LeadRouter(
            string tenantId,
            long boxId,
            IReadOnlyCollection<CampaignBoxNoSql> campaignBoxes,
            ILeadRepository leadRepository,
            IMyNoSqlServerDataReader<LeadRouterNoSqlEntity> dataReader,
            IMyNoSqlServerDataWriter<LeadRouterNoSqlEntity> dataWriter,
            IMyNoSqlServerDataReader<LeadRouterCapacitorBoxNoSqlEntity> capacitorReader,
            IMyNoSqlServerDataWriter<LeadRouterCapacitorBoxNoSqlEntity> capacitorWriter)
        {
            _tenantId = tenantId;
            _leadRepository = leadRepository;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            _capacitorReader = capacitorReader;
            _capacitorWriter = capacitorWriter;
            _campaignBoxes = campaignBoxes.ToArray();
            BoxId = boxId;
        }

        //todo: cap daily cap
        public async Task<CampaignBoxNoSql> GetCampaignBox(string country)
        {
            var date = DateTime.UtcNow;

            var leadRouter = _dataReader.Get(LeadRouterNoSqlEntity.GeneratePartitionKey(_tenantId),
                LeadRouterNoSqlEntity.GenerateRowKey(BoxId));

            var leadsRouted = leadRouter?.NoSqlInfo.LeadsRoutedCount ?? 0;

            var capacitors = _capacitorReader.Get(LeadRouterCapacitorBoxNoSqlEntity.GeneratePartitionKey(BoxId));

            Dictionary<long, LeadRouterCapacitorBoxNoSqlEntity> countDict =
                new Dictionary<long, LeadRouterCapacitorBoxNoSqlEntity>();

            bool saveAll = false;

            if (capacitors == null || !capacitors.Any())
            {
                saveAll = true;
                countDict = _campaignBoxes.ToDictionary(x => x.CampaignBoxId, y => LeadRouterCapacitorBoxNoSqlEntity.Create(new LeadRouteCapacitorNoSqlInfo()
                {
                    BoxId = BoxId,
                    CampaignBoxId = y.CampaignBoxId,
                    ProcessedLeads = 0
                }));
            }
            else
            {
                countDict = capacitors.ToDictionary(x => x.NoSqlInfo.CampaignBoxId);
            }

            List<CampaignBoxNoSql> filtered = new List<CampaignBoxNoSql>(_campaignBoxes.Length);

            foreach (var currentCampaign in _campaignBoxes)
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

                if (activityHours.From.HasValue && date.TimeOfDay < activityHours.From.Value.TimeOfDay)
                {
                    continue;
                }

                if (activityHours.To.HasValue && date.TimeOfDay > activityHours.To.Value.TimeOfDay)
                {
                    continue;
                }

                filtered.Add(currentCampaign);
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
                                BoxId = BoxId,
                                TenantId = _tenantId
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
    }
}