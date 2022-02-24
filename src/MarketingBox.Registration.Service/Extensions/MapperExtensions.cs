using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using System;
using RegistrationAdditionalInfo = MarketingBox.Registration.Service.Messages.Registrations.RegistrationAdditionalInfo;
using RegistrationCustomerInfo = MarketingBox.Registration.Service.Messages.Registrations.RegistrationCustomerInfo;
using RegistrationGeneralInfo = MarketingBox.Registration.Service.Messages.Registrations.RegistrationGeneralInfo;
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
            this Domain.Registrations.Registration registration)
        {
            return new RegistrationRequest()
            {
                TenantId = registration.TenantId,
                RegistrationId = registration.RegistrationInfo.RegistrationId,
                RegistrationUniqueId = registration.RegistrationInfo.RegistrationUid,
                IntegrationName = registration.RouteInfo.Integration,
                IntegrationId = registration.RouteInfo.IntegrationId ?? default,
                AffiliateId = registration.RouteInfo.AffiliateId,
                Info = new Integration.Service.Grpc.Models.Registrations.RegistrationInfo()
                {
                    FirstName = registration.RegistrationInfo.FirstName,
                    LastName = registration.RegistrationInfo.LastName,
                    Email = registration.RegistrationInfo.Email,
                    Ip = registration.RegistrationInfo.Ip,
                    Phone = registration.RegistrationInfo.Phone,
                    Password = registration.RegistrationInfo.Password,
                    Country = registration.RegistrationInfo.Country,
                },
                AdditionalInfo = new Integration.Service.Grpc.Models.Registrations.RegistrationAdditionalInfo()
                {
                    So = registration.AdditionalInfo?.Funnel,
                    Sub = registration.AdditionalInfo?.AffCode,
                    Sub1 = registration.AdditionalInfo?.Sub1,
                    Sub2 = registration.AdditionalInfo?.Sub2,
                    Sub3 = registration.AdditionalInfo?.Sub3,
                    Sub4 = registration.AdditionalInfo?.Sub4,
                    Sub5 = registration.AdditionalInfo?.Sub5,
                    Sub6 = registration.AdditionalInfo?.Sub6,
                    Sub7 = registration.AdditionalInfo?.Sub7,
                    Sub8 = registration.AdditionalInfo?.Sub8,
                    Sub9 = registration.AdditionalInfo?.Sub9,
                    Sub10 = registration.AdditionalInfo?.Sub10,
                },
            };
        }

        public static RegistrationUpdateMessage MapToMessage(this Domain.Registrations.Registration registration)
        {
            return new RegistrationUpdateMessage()
            {
                TenantId = registration.TenantId,
                GeneralInfo = new RegistrationGeneralInfo()
                {
                    Email = registration.RegistrationInfo.Email,
                    FirstName = registration.RegistrationInfo.FirstName,
                    LastName = registration.RegistrationInfo.LastName,
                    Phone = registration.RegistrationInfo.Phone,
                    Ip = registration.RegistrationInfo.Ip,
                    Password = registration.RegistrationInfo.Password,
                    CreatedAt = registration.RegistrationInfo.CreatedAt.UtcDateTime,
                    RegistrationId = registration.RegistrationInfo.RegistrationId,
                    RegistrationUId = registration.RegistrationInfo.RegistrationUid,
                    Country = registration.RegistrationInfo.Country,
                    UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime
                },
                AdditionalInfo = new RegistrationAdditionalInfo()
                {
                    Funnel = registration.AdditionalInfo.Funnel,
                    AffCode = registration.AdditionalInfo.AffCode,
                    Sub1 = registration.AdditionalInfo.Sub1,
                    Sub2 = registration.AdditionalInfo.Sub2,
                    Sub3 = registration.AdditionalInfo.Sub3,
                    Sub4 = registration.AdditionalInfo.Sub4,
                    Sub5 = registration.AdditionalInfo.Sub5,
                    Sub6 = registration.AdditionalInfo.Sub6,
                    Sub7 = registration.AdditionalInfo.Sub7,
                    Sub8 = registration.AdditionalInfo.Sub8,
                    Sub9 = registration.AdditionalInfo.Sub9,
                    Sub10 = registration.AdditionalInfo.Sub10,
                },
                RouteInfo = new RegistrationRouteInfo()
                {
                    AffiliateId = registration.RouteInfo.AffiliateId,
                    CampaignId = registration.RouteInfo.CampaignId,
                    Integration = registration.RouteInfo.Integration,
                    BrandId = registration.RouteInfo.BrandId ?? default,
                    IntegrationId = registration.RouteInfo.IntegrationId ?? default,
                    ConversionDate = registration.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registration.RouteInfo.DepositDate?.UtcDateTime,
                    CrmStatus = registration.RouteInfo.CrmStatus,
                    Status = registration.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                    CustomerInfo = new RegistrationCustomerInfo()
                    {
                        CustomerId = registration.RouteInfo?.CustomerInfo?.CustomerId,
                        LoginUrl = registration.RouteInfo?.CustomerInfo?.LoginUrl,
                        Token = registration.RouteInfo?.CustomerInfo?.Token,
                    },
                    UpdateMode = registration.RouteInfo.UpdateMode.MapEnum<DepositUpdateMode>(),
                    AffiliateName = registration.RouteInfo.AffiliateName,
                    AutologinUsed = registration.RouteInfo.AutologinUsed
                },
            };
        }
    }
}
