using System;
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
        public long RouteInfoAffiliateId { get; set; }
        public long RouteInfoBrandId { get; set; }
        public long RouteInfoCampaignId { get; set; }
        public string RouteInfoIntegration { get; set; }
        public long RouteInfoIntegrationId { get; set; }
        public RegistrationStatus RouteInfoStatus { get; set; }
        public RegistrationApprovedType RouteInfoApprovedType { get; set; }
        public string AdditionalInfoSo { get; set; }
        public string AdditionalInfoSub { get; set; }
        public string AdditionalInfoSub1 { get; set; }
        public string AdditionalInfoSub2 { get; set; }
        public string AdditionalInfoSub3 { get; set; }
        public string AdditionalInfoSub4 { get; set; }
        public string AdditionalInfoSub5 { get; set; }
        public string AdditionalInfoSub6 { get; set; }
        public string AdditionalInfoSub7 { get; set; }
        public string AdditionalInfoSub8 { get; set; }
        public string AdditionalInfoSub9 { get; set; }
        public string AdditionalInfoSub10 { get; set; }
        public string RouteInfoCustomerInfoCustomerId { get; set; }
        public string RouteInfoCustomerInfoToken { get; set; }
        public string RouteInfoCustomerInfoLoginUrl { get; set; }
        public string RouteInfoCustomerInfoBrand { get; set; }
        public string RouteInfoCrmStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? RouteInfoDepositDate { get; set; }
        public DateTimeOffset? RouteInfoConversionDate { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Sequence { get; set; }
    }
}
