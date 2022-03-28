using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Crm;

[DataContract]
public class UpdateCrmStatusRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
    public string TenantId { get; set; }

    [DataMember(Order = 2), Required, StringLength(128, MinimumLength = 1)]
    public string CustomerId { get; set; }

    [DataMember(Order = 3), Required]
    public CrmStatus? Crm { get; set; }
}