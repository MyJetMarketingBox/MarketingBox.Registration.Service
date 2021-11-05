using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.MyNoSql.Registrations;
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
                   request.GeneralInfo.Ip + "_";
        }
        public static RegistrationRequest CreateIntegrationRequest(
            this Domain.Registrations.Registration registration)
        {
            return new RegistrationRequest()
            {
                TenantId = registration.TenantId,
                LeadId = registration.RegistrationInfo.RegistrationId,
                LeadUniqueId = registration.RegistrationInfo.UniqueId,
                BrandName = registration.RouteInfo.Integration,
                BrandId = registration.RouteInfo.IntegrationId,
                Info = new RegistrationLeadInfo()
                {
                    FirstName = registration.RegistrationInfo.FirstName,
                    LastName = registration.RegistrationInfo.LastName,
                    Email = registration.RegistrationInfo.Email,
                    Ip = registration.RegistrationInfo.Ip,
                    Phone = registration.RegistrationInfo.Phone,
                    Password = registration.RegistrationInfo.Password,
                    Country = registration.RegistrationInfo.Country,
                },
                AdditionalInfo = new RegistrationLeadAdditionalInfo()
                {
                    So = registration.AdditionalInfo?.So,
                    Sub = registration.AdditionalInfo?.Sub,
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
                Sequence = registration.Sequence,
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
                    UniqueId = registration.RegistrationInfo.UniqueId,
                    Country = registration.RegistrationInfo.Country,
                    UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime
                },
                AdditionalInfo = new RegistrationAdditionalInfo()
                {
                    So = registration.AdditionalInfo.So,
                    Sub = registration.AdditionalInfo.Sub,
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
                    BrandId = registration.RouteInfo.BrandId,
                    IntegrationId = registration.RouteInfo.IntegrationId,
                    ConversionDate = registration.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registration.RouteInfo.DepositDate?.UtcDateTime,
                    CrmCrmStatus = registration.RouteInfo.CrmStatus,
                    Status = registration.RouteInfo.Status.MapEnum<Messages.Common.LeadStatus>(),
                    CustomerInfo = new RegistrationCustomerInfo()
                    {
                        CustomerId = registration.RouteInfo?.CustomerInfo?.CustomerId,
                        LoginUrl = registration.RouteInfo?.CustomerInfo?.LoginUrl,
                        Token = registration.RouteInfo?.CustomerInfo?.Token,
                    },
                    ApprovedType = registration.RouteInfo.ApprovedType.MapEnum<Messages.Common.LeadApprovedType>(),
                },
            };
        }

        public static RegistrationNoSqlEntity MapToNoSql(this Domain.Registrations.Registration registration)
        {
            return RegistrationNoSqlEntity.Create(
                new RegistrationNoSqlInfo()
                {
                    TenantId = registration.TenantId,
                    Sequence = registration.Sequence,
                    GeneralInfo = new MyNoSql.Registrations.RegistrationGeneralInfo()
                    {
                        RegistrationId = registration.RegistrationInfo.RegistrationId,
                        CreatedAt = registration.RegistrationInfo.CreatedAt.UtcDateTime,
                        Email = registration.RegistrationInfo.Email,
                        
                        UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime,
                        Country = registration.RegistrationInfo.Country,
                        Ip = registration.RegistrationInfo.Ip,
                        FirstName = registration.RegistrationInfo.FirstName,
                        LastName = registration.RegistrationInfo.LastName,
                        Password = registration.RegistrationInfo.Password,
                        Phone = registration.RegistrationInfo.Phone,
                        UniqueId = registration.RegistrationInfo.UniqueId

                    },
                    AdditionalInfo = new MyNoSql.Registrations.RegistrationAdditionalInfo()
                    {
                        So = registration.AdditionalInfo?.So,
                        Sub = registration.AdditionalInfo?.Sub,
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
                    RouteInfo = new MyNoSql.Registrations.RegistrationRouteInfo()
                    {
                        AffiliateId = registration.RouteInfo.AffiliateId,
                        CampaignId = registration.RouteInfo.CampaignId,
                        Integration = registration.RouteInfo.Integration,
                        BrandId = registration.RouteInfo.BrandId,
                        IntegrationId = registration.RouteInfo.IntegrationId,
                        Status = registration.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                        DepositDate = registration.RouteInfo?.DepositDate?.UtcDateTime,
                        ConversionDate = registration.RouteInfo?.ConversionDate?.UtcDateTime,
                        CrmCrmStatus = registration.RouteInfo.CrmStatus,
                        CustomerInfo = new MyNoSql.Registrations.RegistrationCustomerInfo()
                        {
                            CustomerId = registration.RouteInfo?.CustomerInfo?.CustomerId,
                            Token = registration.RouteInfo?.CustomerInfo?.Token,
                            LoginUrl = registration.RouteInfo?.CustomerInfo?.LoginUrl,
                            Brand = registration.RouteInfo?.CustomerInfo?.Brand
                        },
                        ApprovedType = registration.RouteInfo.ApprovedType.MapEnum<RegistrationApprovedType>(),
                    },
                });
        }
    }
}
