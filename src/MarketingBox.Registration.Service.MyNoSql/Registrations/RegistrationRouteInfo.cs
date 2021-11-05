using System;

namespace MarketingBox.Registration.Service.MyNoSql.Registrations
{
    public class RegistrationRouteInfo
    {
        public long AffiliateId { get; set; }
        public long CampaignId { get; set; }
        public long BrandId { get; set; }
        public string Integration { get; set; }
        public long IntegrationId { get; set; }
        public RegistrationStatus Status { get; set; }
        public string CrmCrmStatus { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ConversionDate { get; set; }
        public RegistrationCustomerInfo CustomerInfo { get; set; }
        public RegistrationApprovedType ApprovedType { get; set; }
    }
}