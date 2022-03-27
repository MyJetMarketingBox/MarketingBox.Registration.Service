using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationUpdateRequest : ValidatableEntity
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