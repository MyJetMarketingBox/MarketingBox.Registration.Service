namespace MarketingBox.Registration.Service.Domain.Models.Entities.Registration
{
    public class RegistrationIdGeneratorEntity
    {
        public long RegistrationId { get; set; }
        public string TenantId { get; set; }
        public string GeneratorId { get; set; }
    }
}
