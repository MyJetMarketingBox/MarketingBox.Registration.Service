using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Affiliate.Contracts
{
    [DataContract]
    public class AffiliateAuthRequest
    {
        [DataMember(Order = 1)]
        public AffiliateAuthInfo AuthInfo { get; set; }

    }
}
