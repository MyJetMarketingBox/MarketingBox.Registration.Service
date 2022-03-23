using MarketingBox.Registration.Service.Domain.Crm;
using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Crm;

[DataContract]
public class UpdateCrmStatusRequest
{
    [DataMember(Order = 1)]
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
