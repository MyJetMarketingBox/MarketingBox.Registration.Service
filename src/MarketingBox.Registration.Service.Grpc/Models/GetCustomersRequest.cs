using System;
using System.Runtime.Serialization;
using MarketingBox.Reporting.Service.Grpc.Models;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomersRequest
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public string ApiKey { get; set; }
        [DataMember(Order = 3)] public string TenantId { get; set; }
        [DataMember(Order = 4)] public DateTime From { get; set; }
        [DataMember(Order = 5)] public DateTime To { get; set; }
        [DataMember(Order = 6)] public CustomersReportType Type { get; set; }
    }
}