using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using ReportingRegistrationDetails = MarketingBox.Reporting.Service.Domain.Models.RegistrationDetails;

namespace MarketingBox.Registration.Service.MapperProfiles
{
    public class RegistrationDetailsMapperProfile : Profile
    {
        public RegistrationDetailsMapperProfile()
        {
            CreateMap<ReportingRegistrationDetails, RegistrationDetails>()
                .ForMember(x => x.ApprovedType,
                    x => x.MapFrom(z => z.UpdateMode));
        }
    }
}