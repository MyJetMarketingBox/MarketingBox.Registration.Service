using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationsGetByDateRequest
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public string ApiKey { get; set; }
        [DataMember(Order = 3)] public DateTime From { get; set; }
        [DataMember(Order = 4)] public DateTime To { get; set; }
        [DataMember(Order = 5)] public RegistrationType Type { get; set; }
    }

    [DataContract]
    public enum RegistrationType
    {
        [DataMember(Order = 1)] Registrations,
        [DataMember(Order = 2)] QFTDepositors,  //TODO Rename
        [DataMember(Order = 3)] All
    }
}