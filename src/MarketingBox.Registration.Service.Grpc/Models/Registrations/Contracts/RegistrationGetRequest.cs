using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationGetRequest 
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }
    }
}