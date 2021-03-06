using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Domain.Models.Affiliate
{
    [DataContract]
    public class AffiliateAuthInfo : ValidatableEntity
    {
        [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThan, 0)]
        public long? AffiliateId { get; set; }

        [DataMember(Order = 2), Required, StringLength(128, MinimumLength = 1)]
        public string ApiKey { get; set; }
    }
}