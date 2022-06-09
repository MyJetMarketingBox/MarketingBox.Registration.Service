using MarketingBox.Registration.Service.Domain.Models.TrafficEngine;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.TrafficEngine;

public class BrandCandidateNoSql : MyNoSqlDbEntity
{
    public const string TableName = "marketingbox-traffic-engine-brandcandidate";
    public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
    public static string GenerateRowKey(long brandId) =>
        $"{brandId}";

    public BrandCandidate BrandCandidate { get; set; }
    
    public static BrandCandidateNoSql Create(BrandCandidate brandCandidate) =>
        new()
        {
            PartitionKey = GeneratePartitionKey(brandCandidate.TenantId),
            RowKey = GenerateRowKey(brandCandidate.BrandId),
            BrandCandidate = brandCandidate,
        };
}