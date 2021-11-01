using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.LeadRouter
{
    public class LeadRouterNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-lead-router";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long boxId) =>
            $"{boxId}";

        public LeadRouterNoSqlInfo NoSqlInfo { get; set; }

        public static LeadRouterNoSqlEntity Create(
            LeadRouterNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.TenantId),
                RowKey = GenerateRowKey(noSqlInfo.BoxId),
                NoSqlInfo = noSqlInfo,
            };
    }
}
