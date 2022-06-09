using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;

[DataContract]
public class GetStatusChangeLogRequest 
{
    [DataMember(Order = 1)] public long? UserId { get; set; }
    [DataMember(Order = 2)] public long? RegistrationId { get; set; }
    [DataMember(Order = 3)] public DepositUpdateMode? Mode { get; set; }
    [DataMember(Order = 4)] public string TenantId { get; set; }
}