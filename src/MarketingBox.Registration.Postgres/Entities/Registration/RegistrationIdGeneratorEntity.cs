namespace MarketingBox.Registration.Postgres.Entities.Registration
{
    public class RegistrationIdGeneratorEntity
    {
        public long RegistrationId { get; set; }
        public string TenantId { get; set; }
        public string GeneratorId { get; set; }
    }
}
