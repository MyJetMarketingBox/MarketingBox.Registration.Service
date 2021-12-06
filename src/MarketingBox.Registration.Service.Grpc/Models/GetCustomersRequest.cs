using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomersRequest
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public string ApiKey { get; set; }
        [DataMember(Order = 3)] public DateTime From { get; set; }
        [DataMember(Order = 4)] public DateTime To { get; set; }
        [DataMember(Order = 5)] public CustomerType Type { get; set; }
    }

    [DataContract]
    public enum CustomerType
    {
        [DataMember(Order = 1)] Leads,
        [DataMember(Order = 2)] Deposits,
        [DataMember(Order = 3)] LeadsAndDeposits
    }
}