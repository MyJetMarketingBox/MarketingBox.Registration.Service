using System;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    public class RegistrationGeneralInfoInternal
    {
        public string RegistrationUid { get; set; }
        public long RegistrationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public int CountryId { get; set; }
        public string CountryAlfa2Code { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
