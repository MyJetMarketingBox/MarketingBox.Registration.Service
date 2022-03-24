using System;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    public class RegistrationRouteInfo
    {
        public long AffiliateId { get; set; }
        public long? BrandId { get; set; }
        public long CampaignId { get; set; }
        public long? IntegrationId { get; set; }
        public string Integration { get; set; }
        public RegistrationStatus Status { get; set; }
        public CrmStatus CrmStatus { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public DepositUpdateMode UpdateMode { get; set; }
        public RegistrationBrandInfo BrandInfo { get; set; }
        public string AffiliateName { get; set; }
        public bool AutologinUsed { get; set; }
    }
}