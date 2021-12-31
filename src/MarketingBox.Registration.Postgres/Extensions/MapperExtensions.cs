using MarketingBox.Registration.Postgres.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Registrations;

namespace MarketingBox.Registration.Postgres.Extensions
{
    public static class MapperExtensions
    {
        public static RegistrationEntity CreateRegistrationEntity(
            this Service.Domain.Registrations.Registration registration)
        {
            return  new RegistrationEntity()
            {
                TenantId = registration.TenantId,
                UniqueId = registration.RegistrationInfo.RegistrationUid,
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
                DepositDate = registration.RouteInfo.DepositDate,
                Status = registration.RouteInfo.Status,
                ConversionDate = registration.RouteInfo.ConversionDate,
                CrmStatus = registration.RouteInfo.CrmStatus,
                AffiliateId = registration.RouteInfo.AffiliateId,
                CampaignId = registration.RouteInfo.CampaignId,
                Integration = registration.RouteInfo.Integration,
                BrandId = registration.RouteInfo.BrandId,
                IntegrationId = registration.RouteInfo.IntegrationId,
                ApprovedType = registration.RouteInfo.UpdateMode,
                CustomerId = registration.RouteInfo.CustomerInfo?.CustomerId,
                CustomerLoginUrl = registration.RouteInfo.CustomerInfo?.LoginUrl,
                CustomerToken = registration.RouteInfo.CustomerInfo?.Token,
                CustomerBrand = registration.RouteInfo.CustomerInfo?.Brand,
                Funnel = registration.AdditionalInfo?.Funnel,
                AffCode = registration.AdditionalInfo?.AffCode,
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
                Sequence = registration.Sequence,
                AffiliateName = registration.RouteInfo.AffiliateName,
            };
        }

        public static Service.Domain.Registrations.Registration RestoreRegistration(this RegistrationEntity registrationEntity)
        {
            var registrationInfo = new RegistrationRouteInfo()
            {
                IntegrationId = registrationEntity.IntegrationId,
                BrandId = registrationEntity.BrandId,
                Integration = registrationEntity.CustomerBrand,
                CampaignId = registrationEntity.CampaignId,
                AffiliateId = registrationEntity.AffiliateId,
                ConversionDate = registrationEntity.ConversionDate,
                DepositDate = registrationEntity.DepositDate,
                Status = registrationEntity.Status,
                CrmStatus = registrationEntity.CrmStatus,
                UpdateMode = registrationEntity.ApprovedType,
                CustomerInfo = new RegistrationCustomerInfo()
                {
                    CustomerId = registrationEntity.CustomerId,
                    Token = registrationEntity.CustomerToken,
                    LoginUrl = registrationEntity.CustomerLoginUrl,
                    Brand = registrationEntity.CustomerBrand,
                },
                AffiliateName = registrationEntity.AffiliateName
            };

            var leadAdditionalInfo = new RegistrationAdditionalInfo()
            {
                Funnel = registrationEntity.Funnel,
                AffCode = registrationEntity.AffCode,
                Sub1 = registrationEntity.Sub1,
                Sub2 = registrationEntity.Sub2,
                Sub3 = registrationEntity.Sub3,
                Sub4 = registrationEntity.Sub4,
                Sub5 = registrationEntity.Sub5,
                Sub6 = registrationEntity.Sub6,
                Sub7 = registrationEntity.Sub7,
                Sub8 = registrationEntity.Sub8,
                Sub9 = registrationEntity.Sub9,
                Sub10 = registrationEntity.Sub10,
            };

            var leadGeneralInfo = new RegistrationGeneralInfo()
            {
                RegistrationUid = registrationEntity.UniqueId,
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

            var lead = Service.Domain.Registrations.Registration.Restore(
                registrationEntity.TenantId, 
                registrationEntity.Sequence, 
                leadGeneralInfo,
                registrationInfo, 
                leadAdditionalInfo);

            return lead;
        }
    }
}
