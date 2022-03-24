using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationUpdateRequest
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 2)]
        public RegistrationGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }
    }
}