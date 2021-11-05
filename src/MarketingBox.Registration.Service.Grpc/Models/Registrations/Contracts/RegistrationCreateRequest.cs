using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationCreateRequest
    {
        [DataMember(Order = 1)]
        public RegistrationGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2)]
        public AffiliateAuthInfo AuthInfo { get; set; }

        [DataMember(Order = 3)]
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }
    }
}
