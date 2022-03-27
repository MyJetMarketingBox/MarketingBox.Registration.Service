using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Domain.Models.Common;

namespace MarketingBox.Registration.Service.Domain.Models.Deposit
{
    [DataContract]
    public class Deposit
    {
        [DataMember(Order = 1)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }

        [DataMember(Order = 3)]
        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }

        [DataMember(Order = 4)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [DataMember(Order = 5)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }

        [DataMember(Order = 6)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }

        [DataMember(Order = 7)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 8)]
        public int CountryId { get; set; }

        [DataMember(Order = 9)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 10)]
        public string UniqueId { get; set; }

        [DataMember(Order = 11)]
        public RegistrationStatus Status { get; set; }

        [Obsolete("This property is obsolete. Use CrmStatus instead.", false)]
        [DataMember(Order = 12)]
        public string CrmCrmStatus { get; set; }

        [DataMember(Order = 13)]
        public DateTime? DepositDate { get; set; }

        [DataMember(Order = 14)]
        public DateTime? ConversionDate { get; set; }

        [DataMember(Order = 15)]
        public DateTime UpdatedAt { get; set; }

        [DataMember(Order = 16)]
        public CrmStatus CrmStatus { get; set; }

        [DataMember(Order = 17)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string AffiliateName { get; set; }

        [DataMember(Order = 18)]
        public long AffiliateId { get; set; }
        
        [DataMember(Order = 1)]
        public string TenantId { get; set; }
    }
}
