using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Common;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;

[DataContract]
public class GetStatusChangeLogRequest 
{
    [DataMember(Order = 1)] public long? UserId { get; set; }
    [DataMember(Order = 2)] public long? RegistrationId { get; set; }
    [DataMember(Order = 3)] public UpdateMode? Mode { get; set; }
}