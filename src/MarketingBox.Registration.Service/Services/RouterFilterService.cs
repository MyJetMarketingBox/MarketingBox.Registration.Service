using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Services
{
    public class RouterFilterService : IRouterFilterService
    {
        private readonly IMyNoSqlServerDataReader<CampaignRowNoSql> _campaignRowNoSqlServerDataReader;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ILogger<RouterFilterService> _logger;

        public RouterFilterService(
            IMyNoSqlServerDataReader<CampaignRowNoSql> campaignRowNoSqlServerDataReader,
            IRegistrationRepository registrationRepository,
            ILogger<RouterFilterService> logger)
        {
            _campaignRowNoSqlServerDataReader = campaignRowNoSqlServerDataReader;
            _registrationRepository = registrationRepository;
            _logger = logger;
        }

        public async Task<List<CampaignRowMessage>> GetSuitableRoutes(long campaignId, int countryId)
        {
            var date = DateTime.UtcNow;
            var campaignRows =
                _campaignRowNoSqlServerDataReader.Get(CampaignRowNoSql.GeneratePartitionKey(campaignId));
            if (!campaignRows.Any())
            {
                throw new NotFoundException("Could not find any campaign rows for campaign with id", campaignId);
            }

            var filtered = new List<CampaignRowMessage>(campaignRows.Count);

            foreach (var currentCampaignRow in campaignRows.Select(x => x.CampaignRow))
            {
                if (!currentCampaignRow.EnableTraffic)
                {
                    continue;
                }

                if (currentCampaignRow.Geo is not null &&
                    !currentCampaignRow.Geo.CountryIds.Contains(countryId))
                {
                    continue;
                }

                var activityHours = currentCampaignRow.ActivityHours.Where(x => x.Day == date.DayOfWeek).ToList();
                if (!activityHours.Exists(x => x.IsActive
                                               && x.From.HasValue
                                               && date.TimeOfDay >= x.From.Value
                                               && x.To.HasValue && date.TimeOfDay <= x.To.Value))
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

            _logger.LogInformation("CampaignRows were selected {@Context}", filtered.Count);
            return filtered;
        }
    }
}