using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.RegistrationRouter
{
    public class RegistrationRouterCapacitorBoxNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-registration-router-capacitor";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(long campaignRowId) =>
            $"{campaignRowId}";

        public RegistrationRouteCapacitorNoSqlInfo NoSqlInfo { get; set; }

        public static RegistrationRouterCapacitorBoxNoSqlEntity Create(
            RegistrationRouteCapacitorNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.CampaignId),
                RowKey = GenerateRowKey(noSqlInfo.CampaignRowId),
                NoSqlInfo = noSqlInfo,
            };
    }
}