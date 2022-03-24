using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationDeleteRequest
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }
    }
}
