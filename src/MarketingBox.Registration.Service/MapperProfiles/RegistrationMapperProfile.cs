using System;
using AutoMapper;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Models.Affiliate;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
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
            CreateMap<RegistrationEntity, Domain.Models.Registrations.Registration>()
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.Message,
                    x => x.MapFrom(z => z.CustomerLoginUrl))
                .ForMember(x => x.BrandInfo,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.OriginalData,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.RegistrationUId,
                    x => x.MapFrom(z => z.UniqueId));
            CreateMap<RegistrationEntity, RegistrationBrandInfo>()
                .ForMember(x => x.Brand,
                    x => x.MapFrom(z => z.CustomerBrand))
                .ForMember(x => x.Token,
                    x => x.MapFrom(z => z.CustomerToken))
                .ForMember(x => x.LoginUrl,
                    x => x.MapFrom(z => z.CustomerLoginUrl));
            CreateMap<RegistrationEntity, RegistrationGeneralInfo>()
                .ForMember(x => x.CountryCode,
                    x => x.MapFrom(z => z.Country));

            CreateMap<RegistrationCreateRequest, RegistrationEntity>()
                .IncludeMembers(
                    x => x.AdditionalInfo,
                    x => x.AuthInfo,
                    x => x.GeneralInfo);
            CreateMap<RegistrationAdditionalInfo, RegistrationEntity>();
            CreateMap<AffiliateAuthInfo, RegistrationEntity>();
            CreateMap<RegistrationGeneralInfo, RegistrationEntity>();

            CreateMap<RegistrationEntity, RegistrationUpdateMessage>()
                .ForMember(x => x.AdditionalInfo,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.RouteInfo,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.GeneralInfoInternal,
                    x => x.MapFrom(z => z));
            CreateMap<RegistrationEntity, RegistrationAdditionalInfo>();
            CreateMap<RegistrationEntity, RegistrationRouteInfo>()
                .ForMember(x => x.UpdateMode,
                    x => x.MapFrom(z => z.ApprovedType))
                .ForMember(x => x.BrandInfo,
                    x => x.MapFrom(z => z));
            CreateMap<RegistrationEntity, RegistrationGeneralInfoInternal>()
                .ForMember(x => x.RegistrationUid,
                    x => x.MapFrom(z => z.UniqueId))
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.CountryAlfa2Code,
                    x => x.MapFrom(z => z.Country));

            CreateMap<RegistrationEntity, Deposit>()
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.UpdatedAt,
                    x => x.MapFrom(z => z.UpdatedAt.UtcDateTime))
                .ForMember(x => x.CreatedAt,
                    x => x.MapFrom(z => z.CreatedAt.UtcDateTime))
                .ForMember(x => x.ConversionDate,
                    x => x.MapFrom(z =>
                        z.ConversionDate.HasValue ? z.ConversionDate.Value.UtcDateTime : (DateTime?) null))
                .ForMember(x => x.DepositDate,
                    x => x.MapFrom(z => z.DepositDate.HasValue ? z.DepositDate.Value.UtcDateTime : (DateTime?) null));
            CreateMap<RegistrationEntity, RegistrationRequest>()
                .ForMember(x => x.RegistrationId,
                    x => x.MapFrom(z => z.Id))
                .ForMember(x => x.RegistrationUniqueId,
                    x => x.MapFrom(z => z.UniqueId))
                .ForMember(x => x.Info,
                    x => x.MapFrom(z => z))
                .ForMember(x => x.AdditionalInfo,
                    x => x.MapFrom(z => z));
            CreateMap<RegistrationEntity, Integration.Service.Grpc.Models.Registrations.RegistrationAdditionalInfo>();
            CreateMap<RegistrationEntity, RegistrationInfo>();
        }
    }
}