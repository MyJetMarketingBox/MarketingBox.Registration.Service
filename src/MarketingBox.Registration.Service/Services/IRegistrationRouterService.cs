using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;

namespace MarketingBox.Registration.Service.Services
{
    public interface IRegistrationRouterService
    {
        Task<List<CampaignRowNoSql>> GetSuitableRoutes(long campaignId, string country);

        Task<CampaignRowNoSql> GetCampaignBox(string tenantId, long campaignId, string country,
            List<CampaignRowNoSql> filtered);
    }
}