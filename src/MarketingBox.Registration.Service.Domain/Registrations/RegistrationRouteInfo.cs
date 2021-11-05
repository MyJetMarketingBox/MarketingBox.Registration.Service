using System;

namespace MarketingBox.Registration.Service.Domain.Registrations
{
    public class RegistrationRouteInfo
    {
        public long AffiliateId { get; set; }
        public long BrandId { get; set; }
        public long CampaignId { get; set; }
        public long IntegrationId { get; set; }
        public string Integration { get; set; }
        public RegistrationStatus Status { get; set; }
        public string CrmStatus { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public RegistrationApprovedType ApprovedType { get; set; }
        public RegistrationCustomerInfo CustomerInfo { get; set; }
    }
}