using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Registration.Service.Services.Interfaces
{
    public interface IRegistrationRouterService
    {
        Task<List<CampaignRowMessage>> GetSuitableRoutes(long campaignId, int countryId);

        Task<CampaignRowMessage> GetCampaignRow(string tenantId, long campaignId, List<CampaignRowMessage> filtered);
    }
}