using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Deposits
{
    [DataContract]
    public class DepositCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThan, 0)]
        public long? RegistrationId { get; set; }
    }
}