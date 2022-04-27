using AutoMapper;
using ReportingRegistrationDetails = MarketingBox.Reporting.Service.Domain.Models.Registrations.RegistrationDetails;

namespace MarketingBox.Registration.Service.MapperProfiles
{
    public class RegistrationDetailsMapperProfile : Profile
    {
        public RegistrationDetailsMapperProfile()
        {
            CreateMap<ReportingRegistrationDetails, Domain.Models.Registrations.Registration>()
                .ForMember(x => x.Id, x => x.MapFrom(z => z.RegistrationId))
                .ForMember(x => x.UniqueId, x => x.MapFrom(z => z.RegistrationUid))
                .ForMember(x => x.ApprovedType, x => x.MapFrom(x => x.UpdateMode));
        }
    }
}