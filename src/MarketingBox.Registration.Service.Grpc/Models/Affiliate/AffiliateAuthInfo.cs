using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Affiliate
{
    [DataContract]
    public class AffiliateAuthInfo
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public string ApiKey { get; set; }

        [DataMember(Order = 3)]
        public long CampaignId { get; set; }
    }
}