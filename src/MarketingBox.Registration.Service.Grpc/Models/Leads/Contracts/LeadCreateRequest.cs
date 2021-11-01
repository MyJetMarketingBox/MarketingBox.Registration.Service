using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class LeadCreateRequest
    {
        [DataMember(Order = 1)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2)]
        public AffiliateAuthInfo AuthInfo { get; set; }

        [DataMember(Order = 3)]
        public LeadAdditionalInfo AdditionalInfo { get; set; }
    }
}
