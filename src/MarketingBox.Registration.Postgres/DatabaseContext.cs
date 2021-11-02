using MarketingBox.Registration.Postgres.Entities.Lead;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Postgres
{
    public class DatabaseContext : DbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "lead-service";

        private const string LeadTableName = "leads";

        public DbSet<LeadEntity> Leads { get; set; }

        private const string LeadIdGeneratorTableName = "leadidgenerator";

        public DbSet<LeadIdGeneratorEntity> LeadIdGenerators { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public static ILoggerFactory LoggerFactory { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetEntities(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeadEntity>().ToTable(LeadTableName);
            modelBuilder.Entity<LeadEntity>().HasKey(e => e.LeadId);
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new {e.TenantId, e.LeadId});
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new { e.CreatedAt, e.RouteInfoCampaignId, });
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new { e.RouteInfoDepositDate, e.RouteInfoCampaignId, });
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new { e.RouteInfoStatus });

            modelBuilder.Entity<LeadIdGeneratorEntity>().ToTable(LeadIdGeneratorTableName);
            modelBuilder.Entity<LeadIdGeneratorEntity>().HasKey(e => new { e.TenantId, e.GeneratorId });
            modelBuilder.Entity<LeadIdGeneratorEntity>().Property(p => p.LeadId).ValueGeneratedOnAdd();
            //modelBuilder.Entity<LeadIdGeneratorEntity>().HasIndex(e => new { e.TenantId, GenerateId = e.GeneratorId });
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
