using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Affiliate;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required] public RegistrationGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2), Required] public AffiliateAuthInfo AuthInfo { get; set; }

        [DataMember(Order = 3)] public RegistrationAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 4), Required, IsEnum]
        public RegistrationMode RegistrationMode { get; set; }

        [DataMember(Order = 5), RequiredOnlyIf(nameof(RegistrationMode), RegistrationMode.S2S)]
        public RegistrationBrandInfo BrandInfo { get; set; }
    }
}