namespace MarketingBox.Registration.Service.Domain.Route
{
    public class RouteParameters
    {
        public string TenantId { get; set; }
        public string BrandName { get; set; }
        public long IntegrationId { get; set; }
        public long BrandId { get; set; }
        public long CampaignId { get; set; }
        public long CampaignRowId { get; set; }

    }
}