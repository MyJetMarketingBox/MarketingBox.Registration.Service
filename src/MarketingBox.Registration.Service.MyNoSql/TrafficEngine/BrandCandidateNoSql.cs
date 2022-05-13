using MarketingBox.Registration.Service.Domain.Models.TrafficEngine;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.TrafficEngine;

public class BrandCandidateNoSql : MyNoSqlDbEntity
{
    public const string TableName = "marketingbox-traffic-engine-brandcandidate";
    public static string GeneratePartitionKey() => "states";
    public static string GenerateRowKey(long campaignId) =>
        $"{campaignId}";

    public BrandCandidate BrandCandidate { get; set; }
    
    public static BrandCandidateNoSql Create(BrandCandidate brandCandidate) =>
        new()
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(brandCandidate.BrandId),
            BrandCandidate = brandCandidate,
        };
}