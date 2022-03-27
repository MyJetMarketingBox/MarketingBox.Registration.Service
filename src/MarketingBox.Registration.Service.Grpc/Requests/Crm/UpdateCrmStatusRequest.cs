using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Crm;

[DataContract]
public class UpdateCrmStatusRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required]
    public string TenantId { get; set; }

    [DataMember(Order = 2)]
    public long RegistrationId { get; set; }

    [DataMember(Order = 3)]
    public string RegistrationUniqueId { get; set; }

    [DataMember(Order = 4)]
    public string CustomerId { get; set; }

    [DataMember(Order = 5)]
    public CrmStatus Crm { get; set; }

    [DataMember(Order = 6)]
    public DateTime CrmUpdatedAt { get; set; }

    [DataMember(Order = 7)]
    public string IntegrationName { get; set; }

    [DataMember(Order = 8)]
    public long IntegrationId { get; set; }
}
