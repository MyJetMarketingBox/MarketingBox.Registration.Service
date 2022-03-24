using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Registrations;

namespace MarketingBox.Registration.Service.Messages.Registrations
{
    [DataContract]
    public class RegistrationUpdateMessage
    {
        public const string Topic = "marketing-box-registration-service-registration-update";
        
        [DataMember(Order = 1)]
        public string TenantId { get; set; }
      
        [DataMember(Order = 2)]
        public RegistrationGeneralInfo_notgrpc GeneralInfoNotgrpc { get; set; }

        [DataMember(Order = 3)]
        public RegistrationRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 4)]
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }
    }
}
