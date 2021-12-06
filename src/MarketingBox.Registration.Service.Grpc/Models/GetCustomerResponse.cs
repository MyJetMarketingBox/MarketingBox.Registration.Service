using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomerResponse
    {
        [DataMember(Order = 1)] public CustomerGrpc Customer { get; set; }
        [DataMember(Order = 100)] public Error Error { get; set; }
    }
}