using System;
using MarketingBox.Registration.Service.Domain.Crm;
using MarketingBox.Registration.Service.Domain.Registrations;

namespace MarketingBox.Registration.Postgres.Entities.Registration
{
    public class RegistrationEntity
    {
        public string TenantId { get; set; }
        public string UniqueId { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public string Country { get; set; }
        public long AffiliateId { get; set; }
        public long? BrandId { get; set; }
        public long CampaignId { get; set; }
        public string Integration { get; set; }
        public long? IntegrationId { get; set; }
        public RegistrationStatus Status { get; set; }
        public DepositUpdateMode ApprovedType { get; set; }
        public string Funnel { get; set; }
        public string AffCode { get; set; }
        public string Sub1 { get; set; }
        public string Sub2 { get; set; }
        public string Sub3 { get; set; }
        public string Sub4 { get; set; }
        public string Sub5 { get; set; }
        public string Sub6 { get; set; }
        public string Sub7 { get; set; }
        public string Sub8 { get; set; }
        public string Sub9 { get; set; }
        public string Sub10 { get; set; }
        public string CustomerId { get; set; }
        public string CustomerToken { get; set; }
        public string CustomerLoginUrl { get; set; }
        public string CustomerBrand { get; set; }
        public CrmStatus CrmStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Sequence { get; set; }
        public string AffiliateName { get; set; }
    }
}
