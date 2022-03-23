﻿using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationGetRequest 
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public string ApiKey { get; set; }
        [DataMember(Order = 3)] public string RegistrationUId { get; set; }
    }
}