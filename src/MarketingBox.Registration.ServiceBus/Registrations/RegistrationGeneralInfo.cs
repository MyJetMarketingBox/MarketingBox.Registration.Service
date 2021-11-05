using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Messages.Common;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class RegistrationGeneralInfo
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
        public string Country { get; set; }

        [DataMember(Order = 8)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 9)]
        public string UniqueId { get; set; }
        
        [DataMember(Order = 10)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 11)]
        public DateTime UpdatedAt { get; set; }

    }
}