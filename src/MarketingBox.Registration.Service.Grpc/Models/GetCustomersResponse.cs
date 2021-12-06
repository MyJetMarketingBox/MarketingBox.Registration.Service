using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomersResponse
    {
        [DataMember(Order = 1)] public List<CustomerGrpc> Customers { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}