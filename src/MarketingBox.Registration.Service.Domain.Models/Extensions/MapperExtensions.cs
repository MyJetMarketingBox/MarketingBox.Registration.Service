using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;

namespace MarketingBox.Registration.Service.Domain.Models.Extensions
{
    public static class MapperExtensions
    {
        public static RegistrationEntity CreateRegistrationEntity(
            this Models.Registrations.Registration_nogrpc registrationNogrpc)
        {
            return  new RegistrationEntity()
            {
                TenantId = registrationNogrpc.TenantId,
                UniqueId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid,
                CreatedAt = registrationNogrpc.RegistrationInfoNotgrpc.CreatedAt,
                FirstName = registrationNogrpc.RegistrationInfoNotgrpc.FirstName,
                LastName = registrationNogrpc.RegistrationInfoNotgrpc.LastName,
                Email = registrationNogrpc.RegistrationInfoNotgrpc.Email,
                Ip = registrationNogrpc.RegistrationInfoNotgrpc.Ip,
                Password = registrationNogrpc.RegistrationInfoNotgrpc.Password,
                Phone = registrationNogrpc.RegistrationInfoNotgrpc.Phone,
                CountryId = registrationNogrpc.RegistrationInfoNotgrpc.CountryId,
                Country = registrationNogrpc.RegistrationInfoNotgrpc.CountryAlfa2Code,
                Id = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                UpdatedAt = registrationNogrpc.RegistrationInfoNotgrpc.UpdatedAt,
                DepositDate = registrationNogrpc.RouteInfo.DepositDate,
                Status = registrationNogrpc.RouteInfo.Status,
                ConversionDate = registrationNogrpc.RouteInfo.ConversionDate,
                CrmStatus = registrationNogrpc.RouteInfo.CrmStatus,
                AffiliateId = registrationNogrpc.RouteInfo.AffiliateId,
                CampaignId = registrationNogrpc.RouteInfo.CampaignId,
                BrandId = registrationNogrpc.RouteInfo.BrandId,
                ApprovedType = registrationNogrpc.RouteInfo.UpdateMode,
                CustomerId = registrationNogrpc.RouteInfo.BrandInfo?.CustomerId,
                CustomerLoginUrl = registrationNogrpc.RouteInfo.BrandInfo?.LoginUrl,
                CustomerToken = registrationNogrpc.RouteInfo.BrandInfo?.Token,
                CustomerBrand = registrationNogrpc.RouteInfo.BrandInfo?.Brand,
                Funnel = registrationNogrpc.AdditionalInfo?.Funnel,
                AffCode = registrationNogrpc.AdditionalInfo?.AffCode,
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
                AffiliateName = registrationNogrpc.RouteInfo.AffiliateName,
                AutologinUsed = registrationNogrpc.RouteInfo.AutologinUsed
            };
        }

        public static Models.Registrations.Registration_nogrpc RestoreRegistration(this RegistrationEntity registrationEntity)
        {
            var registrationInfo = new RegistrationRouteInfo()
            {
                BrandId = registrationEntity.BrandId,
                Integration = registrationEntity.CustomerBrand,
                CampaignId = registrationEntity.CampaignId,
                AffiliateId = registrationEntity.AffiliateId,
                ConversionDate = registrationEntity.ConversionDate,
                DepositDate = registrationEntity.DepositDate,
                Status = registrationEntity.Status,
                CrmStatus = registrationEntity.CrmStatus,
                UpdateMode = registrationEntity.ApprovedType,
                BrandInfo = new RegistrationBrandInfo()
                {
                    CustomerId = registrationEntity.CustomerId,
                    Token = registrationEntity.CustomerToken,
                    LoginUrl = registrationEntity.CustomerLoginUrl,
                    Brand = registrationEntity.CustomerBrand,
                },
                AffiliateName = registrationEntity.AffiliateName, 
                AutologinUsed = registrationEntity.AutologinUsed
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

            var leadGeneralInfo = new RegistrationGeneralInfo_notgrpc()
            {
                RegistrationUid = registrationEntity.UniqueId,
                RegistrationId = registrationEntity.Id,
                FirstName = registrationEntity.FirstName,
                LastName = registrationEntity.LastName,
                Password = registrationEntity.Password,
                Email = registrationEntity.Email,
                Phone = registrationEntity.Phone,
                Ip = registrationEntity.Ip,
                CountryId = registrationEntity.CountryId,
                CreatedAt = registrationEntity.CreatedAt,
                UpdatedAt = registrationEntity.UpdatedAt,
            };

            var lead = Models.Registrations.Registration_nogrpc.Restore(
                registrationEntity.TenantId, 
                leadGeneralInfo,
                registrationInfo, 
                leadAdditionalInfo);

            return lead;
        }
    }
}
