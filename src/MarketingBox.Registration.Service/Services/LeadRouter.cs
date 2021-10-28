using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;

namespace MarketingBox.Registration.Service.Services
{
    public class LeadRouter
    {
        private readonly ILeadRepository _leadRepository;
        private readonly CampaignBoxNoSql[] _campaignBoxes;
        private long AffiliateId;
        private long CampaignId;
        public long BoxId { get; }

        public LeadRouter(long boxId, IReadOnlyCollection<CampaignBoxNoSql> campaignBoxes, ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
            _campaignBoxes = campaignBoxes.ToArray();
            BoxId = boxId;
        }

        //todo: cap daily cap
        public async Task<CampaignBoxNoSql> GetCampaignBox(string country)
        {
            var date = DateTime.UtcNow;

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
                    currentCap = await _leadRepository.GetCountForLeads(date, LeadStatus.Registered);
                }
                else if (currentCampaign.CapType == CapType.Ftds)
                {
                    currentCap = await _leadRepository.GetCountForDeposits(date, LeadStatus.Approved);
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

            var priorities = filtered.Select(x => x.Priority).Distinct().OrderBy(x => x).ToArray();
            var ordered = filtered.ToLookup(x => x.Priority);

            //todo finish this routing algo
            foreach (var priority in priorities)
            {
                var samePriority = ordered[priority].OrderByDescending(x => x.Weight).ToArray();
                return samePriority.First();
            }

            return null;
        }
    }
}