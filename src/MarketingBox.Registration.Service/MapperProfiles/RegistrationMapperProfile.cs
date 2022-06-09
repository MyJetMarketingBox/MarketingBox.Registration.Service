using System;
using AutoMapper;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Models.Affiliate;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Registration.Service.Messages.Registrations;
using RegistrationAdditionalInfo =
    MarketingBox.Registration.Service.Domain.Models.Registrations.RegistrationAdditionalInfo;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Domain.Models.Registrations.RegistrationRouteInfo;

namespace MarketingBox.Registration.Service.MapperProfiles
{
    public class RegistrationMapperProfile : Profile
    {
        public RegistrationMapperProfile()
        {
            CreateMap<Domain.Models.Registrations.Registration, RegistrationBrandInfo>()
                .ForMember(x => x.Brand,
                    x => x.MapFrom(z => z.CustomerBrand))
                .ForMember(x => x.Token,
                    x => x.MapFrom(z => z.CustomerToken))
                .ForMember(x => x.LoginUrl,
                    x => x.MapFrom(z => z.CustomerLoginUrl));
            CreateMap<Domain.Models.Registrations.Registration, RegistrationGeneralInfo>()
                .ForMember(x => x.CountryCode,
                    x => x.MapFrom(z => z.Country));

            CreateMap<RegistrationCreateRequest, Domain.Models.Registrations.Registration>()
                .IncludeMembers(
                    x => x.AdditionalInfo,
                    x => x.AuthInfo,
                    x => x.GeneralInfo)
                .ForMember(x => x.CreatedAt, x => x.MapFrom(z => DateTime.UtcNow))
                .ForMember(x => x.UpdatedAt, x => x.MapFrom(z => DateTime.UtcNow));
            CreateMap<RegistrationCreateS2SRequest, Domain.Models.Registrations.Registration>()
                .IncludeMembers(
                    x => x.AdditionalInfo,
                    x => x.AuthInfo,
                    x => x.GeneralInfo,
                    x => x.BrandInfo)
                .ForMember(x => x.CreatedAt, x => x.MapFrom(z => DateTime.UtcNow))
                .ForMember(x => x.UpdatedAt, x => x.MapFrom(z => DateTime.UtcNow));
            CreateMap<RegistrationBrandInfo, Domain.Models.Registrations.Registration>()
                .ForMember(x => x.CustomerId, x => x.MapFrom(z => z.CustomerId))
                .ForMember(x => x.CustomerToken, x => x.MapFrom(z => z.Token))
                .ForMember(x => x.CustomerLoginUrl, x => x.MapFrom(z => z.LoginUrl))
                .ForMember(x => x.CustomerBrand, x => x.MapFrom(z => z.Brand));
            CreateMap<RegistrationAdditionalInfo, Domain.Models.Registrations.Registration>();
            CreateMap<AffiliateAuthInfo, Domain.Models.Registrations.Registration>();
            CreateMap<RegistrationGeneralInfo, Domain.Models.Registrations.Registration>();

            CreateMap<Domain.Models.Registrations.Registration, RegistrationUpdateMessage>()
                .ForMember(x => x.AdditionalInfo,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.RouteInfo,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.GeneralInfoInternal,
                    x => x.MapFrom(z => z));
            CreateMap<Domain.Models.Registrations.Registration, RegistrationAdditionalInfo>();
            CreateMap<Domain.Models.Registrations.Registration, RegistrationRouteInfo>()
                .ForMember(x => x.UpdateMode,
                    x => x.MapFrom(z => z.ApprovedType))
                .ForMember(x => x.BrandInfo,
                    x => x.MapFrom(z => z));
            CreateMap<Domain.Models.Registrations.Registration, RegistrationGeneralInfoInternal>()
                .ForMember(x => x.RegistrationUid,
                    x => x.MapFrom(z => z.UniqueId))
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.CountryAlfa2Code,
                    x => x.MapFrom(z => z.Country));

            CreateMap<Domain.Models.Registrations.Registration, RegistrationRequest>()
                .ForMember(x => x.IntegrationName,
                    x => x.MapFrom(z => z.Integration))
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.RegistrationUniqueId,
                    x => x.MapFrom(z => z.UniqueId))
                .ForMember(x => x.Info,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.AdditionalInfo,
                    x => x.MapFrom(z => z));
            CreateMap<Domain.Models.Registrations.Registration,
                Integration.Service.Grpc.Models.Registrations.RegistrationAdditionalInfo>();
            CreateMap<Domain.Models.Registrations.Registration, RegistrationInfo>();
        }
    }
}