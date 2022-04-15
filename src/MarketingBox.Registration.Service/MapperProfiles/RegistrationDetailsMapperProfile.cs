using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using ReportingRegistrationDetails = MarketingBox.Reporting.Service.Domain.Models.Registrations.RegistrationDetails;

namespace MarketingBox.Registration.Service.MapperProfiles
{
    public class RegistrationDetailsMapperProfile : Profile
    {
        public RegistrationDetailsMapperProfile()
        {
            CreateMap<ReportingRegistrationDetails, RegistrationDetails>();
        }
    }
}