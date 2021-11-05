using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class RegistrationRouteInfo
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public long BoxId { get; set; }

        [DataMember(Order = 3)]
        public long CampaignId { get; set; }

        [DataMember(Order = 4)]
        public string Brand { get; set; }
    }
}


