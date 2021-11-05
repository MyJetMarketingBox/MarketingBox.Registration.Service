namespace MarketingBox.Registration.Service.MyNoSql.Registrations
{
    public class RegistrationNoSqlInfo
    {
        public string TenantId { get; set; }
        public RegistrationGeneralInfo GeneralInfo { get; set; }
        public RegistrationRouteInfo RouteInfo { get; set; }
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }
        public long Sequence { get; set; }
    }
}