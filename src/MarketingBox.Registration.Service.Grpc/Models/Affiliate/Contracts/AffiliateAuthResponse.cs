using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Affiliate.Contracts
{
    [DataContract]
    public class AffiliateAuthResponse
    {
        [DataMember(Order = 1)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}