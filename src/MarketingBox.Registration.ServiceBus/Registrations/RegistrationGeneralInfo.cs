using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Registration.Service.Messages.Registrations
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
        public int CountryId { get; set; }

        [DataMember(Order = 8)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 9)]
        [ObsoleteAttribute("This property is obsolete. Use RegistrationUId instead.", false)]
        public string UniqueId { get; set; }
        
        [DataMember(Order = 10)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 11)]
        public DateTime UpdatedAt { get; set; }

        [DataMember(Order = 12)]
        public string RegistrationUId { get; set; }
    }
}