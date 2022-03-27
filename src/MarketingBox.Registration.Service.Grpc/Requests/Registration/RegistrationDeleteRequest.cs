using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationDeleteRequest : ValidatableEntity
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }
    }
}
