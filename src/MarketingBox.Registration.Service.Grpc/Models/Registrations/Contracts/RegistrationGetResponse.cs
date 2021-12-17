using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationGetResponse
    {
        [DataMember(Order = 1)] public RegistrationDetails Customer { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}