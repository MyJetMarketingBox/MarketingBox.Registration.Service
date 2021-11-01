using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.LeadRouter
{
    public class LeadRouterCapacitorBoxNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-lead-router-capacitor";
        public static string GeneratePartitionKey(long boxId) => $"{boxId}";
        public static string GenerateRowKey(long campaignBoxId) =>
            $"{campaignBoxId}";

        public LeadRouteCapacitorNoSqlInfo NoSqlInfo { get; set; }

        public static LeadRouterCapacitorBoxNoSqlEntity Create(
            LeadRouteCapacitorNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.BoxId),
                RowKey = GenerateRowKey(noSqlInfo.CampaignBoxId),
                NoSqlInfo = noSqlInfo,
            };
    }
}