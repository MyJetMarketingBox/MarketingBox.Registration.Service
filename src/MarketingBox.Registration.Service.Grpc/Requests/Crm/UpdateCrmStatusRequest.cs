using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Crm;

[DataContract]
public class UpdateCrmStatusRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
    public string TenantId { get; set; }

    [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThan, 0)]
    public long? RegistrationId { get; set; }

    [DataMember(Order = 3), Required, IsEnum] public CrmStatus? Crm { get; set; }
}