using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit
{
    [DataContract]
    public class StatusChangeLog
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public DateTime Date { get; set; }
        [DataMember(Order = 3)] public string TenantId { get; set; }
        [DataMember(Order = 4)] public long UserId { get; set; }
        [DataMember(Order = 5)] public string UserName { get; set; }
        [DataMember(Order = 6)] public long RegistrationId { get; set; }
        [DataMember(Order = 7)] public DepositUpdateMode Mode { get; set; }
        [DataMember(Order = 8)] public RegistrationStatus OldStatus { get; set; }
        [DataMember(Order = 9)] public RegistrationStatus NewStatus { get; set; }
        [DataMember(Order = 10)] public string Comment { get; set; }
    }
}