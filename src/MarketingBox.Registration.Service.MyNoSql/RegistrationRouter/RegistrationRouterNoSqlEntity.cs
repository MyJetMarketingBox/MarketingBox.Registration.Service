using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.RegistrationRouter
{
    public class RegistrationRouterNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-registration-router";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long campaignId) =>
            $"{campaignId}";

        public RegistrationRouterNoSqlInfo NoSqlInfo { get; set; }

        public static RegistrationRouterNoSqlEntity Create(
            RegistrationRouterNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.TenantId),
                RowKey = GenerateRowKey(noSqlInfo.CampaignId),
                NoSqlInfo = noSqlInfo,
            };
    }
}
