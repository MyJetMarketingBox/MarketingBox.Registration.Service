using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    [DataContract]
    public class RegistrationDetails
    {
        [DataMember(Order = 1)] public string RegistrationUid { get; set; }
        [DataMember(Order = 2)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 3)] public string TenantId { get; set; }
        [DataMember(Order = 4)] public string FirstName { get; set; }
        [DataMember(Order = 5)] public string LastName { get; set; }
        [DataMember(Order = 6)] public string Email { get; set; }
        [DataMember(Order = 7)] public string Phone { get; set; }
        [DataMember(Order = 8)] public string Ip { get; set; }
        [DataMember(Order = 9)] public string Country { get; set; }
        [DataMember(Order = 10)] public long AffiliateId { get; set; }
        [DataMember(Order = 11)] public long BrandId { get; set; }
        [DataMember(Order = 12)] public long CampaignId { get; set; }
        [DataMember(Order = 13)] public string AffiliateName { get; set; }
        [DataMember(Order = 14)] public DateTime? ConversionDate { get; set; }
        [DataMember(Order = 15)] public DateTime? DepositDate { get; set; }
        [DataMember(Order = 16)] public CrmStatus CrmStatus { get; set; }
        [DataMember(Order = 17)] public long RegistrationId { get; set; }
        [DataMember(Order = 18)] public string Integration { get; set; }
        [DataMember(Order = 19)] public long IntegrationId { get; set; }
        [DataMember(Order = 20)] public RegistrationStatus Status { get; set; }
        [DataMember(Order = 21)] public DepositUpdateMode UpdateMode { get; set; }
        [DataMember(Order = 22)] public string Funnel { get; set; }
        [DataMember(Order = 23)] public string AffCode { get; set; }
        [DataMember(Order = 24)] public string Sub1 { get; set; }
        [DataMember(Order = 25)] public string Sub2 { get; set; }
        [DataMember(Order = 26)] public string Sub3 { get; set; }
        [DataMember(Order = 27)] public string Sub4 { get; set; }
        [DataMember(Order = 28)] public string Sub5 { get; set; }
        [DataMember(Order = 29)] public string Sub6 { get; set; }
        [DataMember(Order = 30)] public string Sub7 { get; set; }
        [DataMember(Order = 31)] public string Sub8 { get; set; }
        [DataMember(Order = 32)] public string Sub9 { get; set; }
        [DataMember(Order = 33)] public string Sub10 { get; set; }
        [DataMember(Order = 34)] public string CustomerId { get; set; }
        [DataMember(Order = 35)] public string CustomerToken { get; set; }
        [DataMember(Order = 36)] public string CustomerLoginUrl { get; set; }
        [DataMember(Order = 37)] public string CustomerBrand { get; set; }
    }
}