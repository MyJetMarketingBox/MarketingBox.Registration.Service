using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Reporting.Service.Domain.Models;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomerResponse
    {
        [DataMember(Order = 1)] public Customer Customer { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}