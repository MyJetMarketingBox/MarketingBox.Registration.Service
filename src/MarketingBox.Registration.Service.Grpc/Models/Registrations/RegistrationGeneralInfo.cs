using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Domain.Models;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations
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
        public CountryCodeType CountryCodeType { get; set; }
        
        [DataMember(Order = 8)]
        public string CountryCode { get; set; }
    }
}