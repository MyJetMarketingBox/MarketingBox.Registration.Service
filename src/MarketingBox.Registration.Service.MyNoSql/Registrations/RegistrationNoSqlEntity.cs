using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.Registrations
{
    public class RegistrationNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-registrations";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long registrationId) =>
            $"{registrationId}";

        public RegistrationNoSqlInfo NoSqlInfo { get; set; }

        public static RegistrationNoSqlEntity Create(
            RegistrationNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.TenantId),
                RowKey = GenerateRowKey(noSqlInfo.GeneralInfo.RegistrationId),
                NoSqlInfo = noSqlInfo,
            };

    }
}
