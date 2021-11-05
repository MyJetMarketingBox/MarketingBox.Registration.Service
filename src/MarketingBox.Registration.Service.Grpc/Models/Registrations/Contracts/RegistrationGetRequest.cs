using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class RegistrationGetRequest 
    {
        [DataMember(Order = 1)]
        public long RegistrationId { get; set; }
    }
}