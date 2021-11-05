using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Postgres.Extensions
{
    public static class MapperExtensions
    {
        public static RegistrationEntity CreateRegistrationEntity(
            this Service.Domain.Leads.Registration registration)
        {
            return  new RegistrationEntity()
            {
                TenantId = registration.TenantId,
                UniqueId = registration.RegistrationInfo.UniqueId,
                CreatedAt = registration.RegistrationInfo.CreatedAt,
                FirstName = registration.RegistrationInfo.FirstName,
                LastName = registration.RegistrationInfo.LastName,
                Email = registration.RegistrationInfo.Email,
                Ip = registration.RegistrationInfo.Ip,
                Password = registration.RegistrationInfo.Password,
                Phone = registration.RegistrationInfo.Phone,
                Country = registration.RegistrationInfo.Country,
                Id = registration.RegistrationInfo.RegistrationId,
                UpdatedAt = registration.RegistrationInfo.UpdatedAt,
                RouteInfoDepositDate = registration.RouteInfo.DepositDate,
                RouteInfoStatus = registration.RouteInfo.Status,
                RouteInfoConversionDate = registration.RouteInfo.ConversionDate,
                RouteInfoCrmStatus = registration.RouteInfo.CrmStatus,
                RouteInfoAffiliateId = registration.RouteInfo.AffiliateId,
                RouteInfoCampaignId = registration.RouteInfo.CampaignId,
                RouteInfoIntegration = registration.RouteInfo.Integration,
                RouteInfoBrandId = registration.RouteInfo.BrandId,
                RouteInfoIntegrationId = registration.RouteInfo.IntegrationId,
                RouteInfoApprovedType = registration.RouteInfo.ApprovedType,
                RouteInfoCustomerInfoCustomerId = registration.RouteInfo.CustomerInfo?.CustomerId,
                RouteInfoCustomerInfoLoginUrl = registration.RouteInfo.CustomerInfo?.LoginUrl,
                RouteInfoCustomerInfoToken = registration.RouteInfo.CustomerInfo?.Token,
                RouteInfoCustomerInfoBrand = registration.RouteInfo.CustomerInfo?.Brand,
                AdditionalInfoSo = registration.AdditionalInfo?.So,
                AdditionalInfoSub = registration.AdditionalInfo?.Sub,
                AdditionalInfoSub1 = registration.AdditionalInfo?.Sub1,
                AdditionalInfoSub2 = registration.AdditionalInfo?.Sub2,
                AdditionalInfoSub3 = registration.AdditionalInfo?.Sub3,
                AdditionalInfoSub4 = registration.AdditionalInfo?.Sub4,
                AdditionalInfoSub5 = registration.AdditionalInfo?.Sub5,
                AdditionalInfoSub6 = registration.AdditionalInfo?.Sub6,
                AdditionalInfoSub7 = registration.AdditionalInfo?.Sub7,
                AdditionalInfoSub8 = registration.AdditionalInfo?.Sub8,
                AdditionalInfoSub9 = registration.AdditionalInfo?.Sub9,
                AdditionalInfoSub10 = registration.AdditionalInfo?.Sub10,
                Sequence = registration.Sequence,
            };
        }

        public static Service.Domain.Leads.Registration RestoreRegistration(this RegistrationEntity registrationEntity)
        {
            var leadBrandRegistrationInfo = new RegistrationRouteInfo()
            {
                IntegrationId = registrationEntity.RouteInfoIntegrationId,
                BrandId = registrationEntity.RouteInfoBrandId,
                Integration = registrationEntity.RouteInfoCustomerInfoBrand,
                CampaignId = registrationEntity.RouteInfoCampaignId,
                AffiliateId = registrationEntity.RouteInfoAffiliateId,
                ConversionDate = registrationEntity.RouteInfoConversionDate,
                DepositDate = registrationEntity.RouteInfoDepositDate,
                Status = registrationEntity.RouteInfoStatus,
                CrmStatus = registrationEntity.RouteInfoCrmStatus,
                ApprovedType = registrationEntity.RouteInfoApprovedType,
                CustomerInfo = new RegistrationCustomerInfo()
                {
                    CustomerId = registrationEntity.RouteInfoCustomerInfoCustomerId,
                    Token = registrationEntity.RouteInfoCustomerInfoToken,
                    LoginUrl = registrationEntity.RouteInfoCustomerInfoLoginUrl,
                    Brand = registrationEntity.RouteInfoCustomerInfoBrand,
                },
            };

            var leadAdditionalInfo = new RegistrationAdditionalInfo()
            {
                So = registrationEntity.AdditionalInfoSo,
                Sub = registrationEntity.AdditionalInfoSub,
                Sub1 = registrationEntity.AdditionalInfoSub1,
                Sub2 = registrationEntity.AdditionalInfoSub2,
                Sub3 = registrationEntity.AdditionalInfoSub3,
                Sub4 = registrationEntity.AdditionalInfoSub4,
                Sub5 = registrationEntity.AdditionalInfoSub5,
                Sub6 = registrationEntity.AdditionalInfoSub6,
                Sub7 = registrationEntity.AdditionalInfoSub7,
                Sub8 = registrationEntity.AdditionalInfoSub8,
                Sub9 = registrationEntity.AdditionalInfoSub9,
                Sub10 = registrationEntity.AdditionalInfoSub10,
            };

            var leadGeneralInfo = new RegistrationGeneralInfo()
            {
                UniqueId = registrationEntity.UniqueId,
                RegistrationId = registrationEntity.Id,
                FirstName = registrationEntity.FirstName,
                LastName = registrationEntity.LastName,
                Password = registrationEntity.Password,
                Email = registrationEntity.Email,
                Phone = registrationEntity.Phone,
                Ip = registrationEntity.Ip,
                Country = registrationEntity.Country,
                CreatedAt = registrationEntity.CreatedAt,
                UpdatedAt = registrationEntity.UpdatedAt,
            };

            var lead = Service.Domain.Leads.Registration.Restore(
                registrationEntity.TenantId, 
                registrationEntity.Sequence, 
                leadGeneralInfo,
                leadBrandRegistrationInfo, 
                leadAdditionalInfo);

            return lead;
        }
    }
}
