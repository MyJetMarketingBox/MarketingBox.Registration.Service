using System;
using System.Runtime.Serialization;
using MarketingBox.Reporting.Service.Grpc.Models;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomersRequest
    {
        [DataMember(Order = 1)] public DateTime From { get; set; }
        [DataMember(Order = 2)] public DateTime To { get; set; }
        [DataMember(Order = 3)] public CustomersReportType Type { get; set; }
    }
}