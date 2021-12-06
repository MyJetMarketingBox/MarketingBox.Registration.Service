using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class GetCustomerRequest
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public string ApiKey { get; set; }
        [DataMember(Order = 3)] public string TenantId { get; set; }
        [DataMember(Order = 4)] public string UId { get; set; }
    }
}