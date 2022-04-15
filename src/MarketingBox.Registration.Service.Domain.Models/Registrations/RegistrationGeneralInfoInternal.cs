using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    [DataContract]
    public class RegistrationGeneralInfoInternal
    {
        [DataMember(Order = 1)] public string RegistrationUid { get; set; }
        [DataMember(Order = 2)] public long RegistrationId { get; set; }
        [DataMember(Order = 3)] public string FirstName { get; set; }
        [DataMember(Order = 4)] public string LastName { get; set; }
        [DataMember(Order = 5)] public string Password { get; set; }
        [DataMember(Order = 6)] public string Email { get; set; }
        [DataMember(Order = 7)] public string Phone { get; set; }
        [DataMember(Order = 8)] public string Ip { get; set; }
        [DataMember(Order = 9)] public int CountryId { get; set; }
        [DataMember(Order = 10)] public string CountryAlfa2Code { get; set; }
        [DataMember(Order = 11)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 12)] public DateTime UpdatedAt { get; set; }
    }
}