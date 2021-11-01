using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using LeadAdditionalInfo = MarketingBox.Registration.Service.Messages.Leads.LeadAdditionalInfo;
using LeadGeneralInfo = MarketingBox.Registration.Service.Messages.Leads.LeadGeneralInfo;
using LeadRouteInfo = MarketingBox.Registration.Service.Messages.Leads.LeadRouteInfo;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {

        public static string GeneratorId(this LeadCreateRequest request)
        {
            return request.GeneralInfo.Email + "_" +
                   request.GeneralInfo.FirstName + "_" +
                   request.GeneralInfo.LastName + "_" +
                   request.GeneralInfo.Ip + "_";
        }
        public static RegistrationRequest CreateIntegrationRequest(
            this Lead lead)
        {
            return new RegistrationRequest()
            {
                TenantId = lead.TenantId,
                LeadId = lead.LeadInfo.LeadId,
                LeadUniqueId = lead.LeadInfo.UniqueId,
                BrandName = lead.RouteInfo.Brand,
                BrandId = lead.RouteInfo.BrandId,
                Info = new RegistrationLeadInfo()
                {
                    FirstName = lead.LeadInfo.FirstName,
                    LastName = lead.LeadInfo.LastName,
                    Email = lead.LeadInfo.Email,
                    Ip = lead.LeadInfo.Ip,
                    Phone = lead.LeadInfo.Phone,
                    Password = lead.LeadInfo.Password,
                    Country = lead.LeadInfo.Country,
                },
                AdditionalInfo = new RegistrationLeadAdditionalInfo()
                {
                    So = lead.AdditionalInfo?.So,
                    Sub = lead.AdditionalInfo?.Sub,
                    Sub1 = lead.AdditionalInfo?.Sub1,
                    Sub2 = lead.AdditionalInfo?.Sub2,
                    Sub3 = lead.AdditionalInfo?.Sub3,
                    Sub4 = lead.AdditionalInfo?.Sub4,
                    Sub5 = lead.AdditionalInfo?.Sub5,
                    Sub6 = lead.AdditionalInfo?.Sub6,
                    Sub7 = lead.AdditionalInfo?.Sub7,
                    Sub8 = lead.AdditionalInfo?.Sub8,
                    Sub9 = lead.AdditionalInfo?.Sub9,
                    Sub10 = lead.AdditionalInfo?.Sub10,
                },
            };
        }

        public static LeadUpdateMessage MapToMessage(this Lead lead)
        {
            return new LeadUpdateMessage()
            {
                TenantId = lead.TenantId,
                Sequence = lead.Sequence,
                GeneralInfo = new LeadGeneralInfo()
                {
                    Email = lead.LeadInfo.Email,
                    FirstName = lead.LeadInfo.FirstName,
                    LastName = lead.LeadInfo.LastName,
                    Phone = lead.LeadInfo.Phone,
                    Ip = lead.LeadInfo.Ip,
                    Password = lead.LeadInfo.Password,
                    CreatedAt = lead.LeadInfo.CreatedAt.UtcDateTime,
                    LeadId = lead.LeadInfo.LeadId,
                    UniqueId = lead.LeadInfo.UniqueId,
                    Country = lead.LeadInfo.Country,
                    UpdatedAt = lead.LeadInfo.UpdatedAt.UtcDateTime
                },
                AdditionalInfo = new LeadAdditionalInfo()
                {
                    So = lead.AdditionalInfo.So,
                    Sub = lead.AdditionalInfo.Sub,
                    Sub1 = lead.AdditionalInfo.Sub1,
                    Sub2 = lead.AdditionalInfo.Sub2,
                    Sub3 = lead.AdditionalInfo.Sub3,
                    Sub4 = lead.AdditionalInfo.Sub4,
                    Sub5 = lead.AdditionalInfo.Sub5,
                    Sub6 = lead.AdditionalInfo.Sub6,
                    Sub7 = lead.AdditionalInfo.Sub7,
                    Sub8 = lead.AdditionalInfo.Sub8,
                    Sub9 = lead.AdditionalInfo.Sub9,
                    Sub10 = lead.AdditionalInfo.Sub10,
                },
                RouteInfo = new LeadRouteInfo()
                {
                    AffiliateId = lead.RouteInfo.AffiliateId,
                    BoxId = lead.RouteInfo.BoxId,
                    Brand = lead.RouteInfo.Brand,
                    CampaignId = lead.RouteInfo.CampaignId,
                    BrandId = lead.RouteInfo.BrandId,
                    ConversionDate = lead.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = lead.RouteInfo.DepositDate?.UtcDateTime,
                    CrmCrmStatus = lead.RouteInfo.CrmStatus,
                    Status = lead.RouteInfo.Status.MapEnum<Messages.Common.LeadStatus>(),
                    CustomerInfo = new Messages.Leads.LeadCustomerInfo()
                    {
                        CustomerId = lead.RouteInfo?.CustomerInfo?.CustomerId,
                        LoginUrl = lead.RouteInfo?.CustomerInfo?.LoginUrl,
                        Token = lead.RouteInfo?.CustomerInfo?.Token,
                    },
                    ApprovedType = lead.RouteInfo.ApprovedType.MapEnum<Messages.Common.LeadApprovedType>(),
                },
            };
        }

        public static LeadNoSqlEntity MapToNoSql(this Lead lead)
        {
            return LeadNoSqlEntity.Create(
                new MyNoSql.Leads.LeadNoSqlInfo()
                {
                    TenantId = lead.TenantId,
                    Sequence = lead.Sequence,
                    GeneralInfo = new MyNoSql.Leads.LeadGeneralInfo()
                    {
                        LeadId = lead.LeadInfo.LeadId,
                        CreatedAt = lead.LeadInfo.CreatedAt.UtcDateTime,
                        Email = lead.LeadInfo.Email,
                        
                        UpdatedAt = lead.LeadInfo.UpdatedAt.UtcDateTime,
                        Country = lead.LeadInfo.Country,
                        Ip = lead.LeadInfo.Ip,
                        FirstName = lead.LeadInfo.FirstName,
                        LastName = lead.LeadInfo.LastName,
                        Password = lead.LeadInfo.Password,
                        Phone = lead.LeadInfo.Phone,
                        UniqueId = lead.LeadInfo.UniqueId

                    },
                    AdditionalInfo = new MyNoSql.Leads.LeadAdditionalInfo()
                    {
                        So = lead.AdditionalInfo?.So,
                        Sub = lead.AdditionalInfo?.Sub,
                        Sub1 = lead.AdditionalInfo?.Sub1,
                        Sub2 = lead.AdditionalInfo?.Sub2,
                        Sub3 = lead.AdditionalInfo?.Sub3,
                        Sub4 = lead.AdditionalInfo?.Sub4,
                        Sub5 = lead.AdditionalInfo?.Sub5,
                        Sub6 = lead.AdditionalInfo?.Sub6,
                        Sub7 = lead.AdditionalInfo?.Sub7,
                        Sub8 = lead.AdditionalInfo?.Sub8,
                        Sub9 = lead.AdditionalInfo?.Sub9,
                        Sub10 = lead.AdditionalInfo?.Sub10,
                    },
                    RouteInfo = new MyNoSql.Leads.LeadRouteInfo()
                    {
                        AffiliateId = lead.RouteInfo.AffiliateId,
                        BoxId = lead.RouteInfo.BoxId,
                        Brand = lead.RouteInfo.Brand,
                        CampaignId = lead.RouteInfo.CampaignId,
                        BrandId = lead.RouteInfo.BrandId,
                        Status = lead.RouteInfo.Status.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadStatus>(),
                        DepositDate = lead.RouteInfo?.DepositDate?.UtcDateTime,
                        ConversionDate = lead.RouteInfo?.ConversionDate?.UtcDateTime,
                        CrmCrmStatus = lead.RouteInfo.CrmStatus,
                        CustomerInfo = new MyNoSql.Leads.LeadCustomerInfo()
                        {
                            CustomerId = lead.RouteInfo?.CustomerInfo?.CustomerId,
                            Token = lead.RouteInfo?.CustomerInfo?.Token,
                            LoginUrl = lead.RouteInfo?.CustomerInfo?.LoginUrl,
                            Brand = lead.RouteInfo?.CustomerInfo?.Brand
                        },
                        ApprovedType = lead.RouteInfo.ApprovedType.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadApprovedType>(),
                    },
                });
        }
    }
}
