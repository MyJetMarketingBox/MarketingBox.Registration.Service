using System;

namespace MarketingBox.Registration.Service.Domain.Registrations
{
    public class RegistrationGeneralInfo
    {
        public string UniqueId { get; set; }
        public long RegistrationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public string Country { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
