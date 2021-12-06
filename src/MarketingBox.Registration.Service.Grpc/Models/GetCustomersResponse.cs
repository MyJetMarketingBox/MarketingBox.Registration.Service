using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Reporting.Service.Domain.Models;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomersResponse
    {
        [DataMember(Order = 1)] public List<Customer> Customers { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}