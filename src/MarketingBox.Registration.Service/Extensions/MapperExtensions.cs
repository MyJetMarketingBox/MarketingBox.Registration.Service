using System;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Messages.Registrations.RegistrationRouteInfo;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {

        public static string GeneratorId(this RegistrationCreateRequest request)
        {
            return request.GeneralInfo.Email + "_" +
                   request.GeneralInfo.FirstName + "_" +
                   request.GeneralInfo.LastName + "_" +
                   request.GeneralInfo.Ip + "_" +
                   DateTime.UtcNow.ToString("O");
        }
        public static RegistrationRequest CreateIntegrationRequest(
            this Domain.Models.Registrations.Registration_nogrpc registrationNogrpc)
        {
            return new RegistrationRequest()
            {
                TenantId = registrationNogrpc.TenantId,
                RegistrationId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                RegistrationUniqueId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid,
                IntegrationName = registrationNogrpc.RouteInfo.Integration,
                IntegrationId = registrationNogrpc.RouteInfo.IntegrationId ?? default,
                AffiliateId = registrationNogrpc.RouteInfo.AffiliateId,
                Info = new Integration.Service.Grpc.Models.Registrations.RegistrationInfo()
                {
                    FirstName = registrationNogrpc.RegistrationInfoNotgrpc.FirstName,
                    LastName = registrationNogrpc.RegistrationInfoNotgrpc.LastName,
                    Email = registrationNogrpc.RegistrationInfoNotgrpc.Email,
                    Ip = registrationNogrpc.RegistrationInfoNotgrpc.Ip,
                    Phone = registrationNogrpc.RegistrationInfoNotgrpc.Phone,
                    Password = registrationNogrpc.RegistrationInfoNotgrpc.Password,
                    Country = registrationNogrpc.RegistrationInfoNotgrpc.CountryAlfa2Code,
                },
                AdditionalInfo = new Integration.Service.Grpc.Models.Registrations.RegistrationAdditionalInfo()
                {
                    So = registrationNogrpc.AdditionalInfo?.Funnel,
                    Sub = registrationNogrpc.AdditionalInfo?.AffCode,
                    Sub1 = registrationNogrpc.AdditionalInfo?.Sub1,
                    Sub2 = registrationNogrpc.AdditionalInfo?.Sub2,
                    Sub3 = registrationNogrpc.AdditionalInfo?.Sub3,
                    Sub4 = registrationNogrpc.AdditionalInfo?.Sub4,
                    Sub5 = registrationNogrpc.AdditionalInfo?.Sub5,
                    Sub6 = registrationNogrpc.AdditionalInfo?.Sub6,
                    Sub7 = registrationNogrpc.AdditionalInfo?.Sub7,
                    Sub8 = registrationNogrpc.AdditionalInfo?.Sub8,
                    Sub9 = registrationNogrpc.AdditionalInfo?.Sub9,
                    Sub10 = registrationNogrpc.AdditionalInfo?.Sub10,
                },
            };
        }

        public static RegistrationUpdateMessage MapToMessage(this Domain.Models.Registrations.Registration_nogrpc registrationNogrpc)
        {
            return new RegistrationUpdateMessage()
            {
                TenantId = registrationNogrpc.TenantId,
                GeneralInfoNotgrpc = new RegistrationGeneralInfo_notgrpc()
                {
                    Email = registrationNogrpc.RegistrationInfoNotgrpc.Email,
                    FirstName = registrationNogrpc.RegistrationInfoNotgrpc.FirstName,
                    LastName = registrationNogrpc.RegistrationInfoNotgrpc.LastName,
                    Phone = registrationNogrpc.RegistrationInfoNotgrpc.Phone,
                    Ip = registrationNogrpc.RegistrationInfoNotgrpc.Ip,
                    Password = registrationNogrpc.RegistrationInfoNotgrpc.Password,
                    CreatedAt = registrationNogrpc.RegistrationInfoNotgrpc.CreatedAt.UtcDateTime,
                    RegistrationId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                    RegistrationUid = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid,
                    CountryId = registrationNogrpc.RegistrationInfoNotgrpc.CountryId,
                    UpdatedAt = registrationNogrpc.RegistrationInfoNotgrpc.UpdatedAt.UtcDateTime
                },
                AdditionalInfo = new RegistrationAdditionalInfo()
                {
                    Funnel = registrationNogrpc.AdditionalInfo.Funnel,
                    AffCode = registrationNogrpc.AdditionalInfo.AffCode,
                    Sub1 = registrationNogrpc.AdditionalInfo.Sub1,
                    Sub2 = registrationNogrpc.AdditionalInfo.Sub2,
                    Sub3 = registrationNogrpc.AdditionalInfo.Sub3,
                    Sub4 = registrationNogrpc.AdditionalInfo.Sub4,
                    Sub5 = registrationNogrpc.AdditionalInfo.Sub5,
                    Sub6 = registrationNogrpc.AdditionalInfo.Sub6,
                    Sub7 = registrationNogrpc.AdditionalInfo.Sub7,
                    Sub8 = registrationNogrpc.AdditionalInfo.Sub8,
                    Sub9 = registrationNogrpc.AdditionalInfo.Sub9,
                    Sub10 = registrationNogrpc.AdditionalInfo.Sub10,
                },
                RouteInfo = new RegistrationRouteInfo()
                {
                    AffiliateId = registrationNogrpc.RouteInfo.AffiliateId,
                    CampaignId = registrationNogrpc.RouteInfo.CampaignId,
                    Integration = registrationNogrpc.RouteInfo.Integration,
                    BrandId = registrationNogrpc.RouteInfo.BrandId ?? default,
                    IntegrationId = registrationNogrpc.RouteInfo.IntegrationId ?? default,
                    ConversionDate = registrationNogrpc.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registrationNogrpc.RouteInfo.DepositDate?.UtcDateTime,
                    CrmStatus = registrationNogrpc.RouteInfo.CrmStatus,
                    Status = registrationNogrpc.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                    BrandInfo = new RegistrationBrandInfo()
                    {
                        CustomerId = registrationNogrpc.RouteInfo?.BrandInfo?.CustomerId,
                        LoginUrl = registrationNogrpc.RouteInfo?.BrandInfo?.LoginUrl,
                        Token = registrationNogrpc.RouteInfo?.BrandInfo?.Token,
                    },
                    UpdateMode = registrationNogrpc.RouteInfo.UpdateMode.MapEnum<DepositUpdateMode>(),
                    AffiliateName = registrationNogrpc.RouteInfo.AffiliateName,
                    AutologinUsed = registrationNogrpc.RouteInfo.AutologinUsed
                },
            };
        }
    }
}
