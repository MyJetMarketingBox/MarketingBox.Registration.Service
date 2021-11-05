using System;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class RegistrationIdGeneratorEntity
    {
        public long RegistrationId { get; set; }
        public string TenantId { get; set; }
        public string GeneratorId { get; set; }
    }
}
