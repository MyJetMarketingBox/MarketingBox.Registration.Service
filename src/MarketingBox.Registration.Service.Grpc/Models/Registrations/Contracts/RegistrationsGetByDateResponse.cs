using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationsGetByDateResponse
    {
        [DataMember(Order = 1)] public List<RegistrationDetails> Customers { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}