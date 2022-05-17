using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;

namespace MarketingBox.Registration.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "registration-service";

        private const string RegistrationTableName = "registrations";
        private const string StatusChangeLogTableName = "status-change-log";
        private const string RegistrationIdGeneratorTableName = "registration_id_generator";

        public DbSet<Service.Domain.Models.Registrations.Registration> Registrations { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }

        public DbSet<RegistrationIdGeneratorEntity> RegistrationIdGenerators { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

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

            SetRegistrationEntity(modelBuilder);
            SetStatusChangeLogEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetStatusChangeLogEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusChangeLog>().ToTable(StatusChangeLogTableName);
            
            modelBuilder.Entity<StatusChangeLog>().HasKey(e => e.Id);
            modelBuilder.Entity<StatusChangeLog>().Property(e => e.Id).UseIdentityColumn();
            
            modelBuilder.Entity<StatusChangeLog>().HasIndex(e => e.Mode);
            modelBuilder.Entity<StatusChangeLog>().HasIndex(e => new {e.TenantId, e.Id});
            modelBuilder.Entity<StatusChangeLog>().HasIndex(e => e.UserId);
            modelBuilder.Entity<StatusChangeLog>().HasIndex(e => e.RegistrationId);
        }

        private void SetRegistrationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().ToTable(RegistrationTableName);
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasKey(e => e.Id);
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasIndex(e => new {e.TenantId, e.Id});
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasIndex(e => new {e.TenantId, e.Email, e.BrandId}).IsUnique();
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasIndex(e => new { e.CreatedAt, e.BrandId, });
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasIndex(e => new { e.DepositDate, e.BrandId, });
            modelBuilder.Entity<Service.Domain.Models.Registrations.Registration>().HasIndex(e => new { e.Status });

            modelBuilder.Entity<RegistrationIdGeneratorEntity>().ToTable(RegistrationIdGeneratorTableName);
            modelBuilder.Entity<RegistrationIdGeneratorEntity>().HasKey(e => new { e.TenantId, e.GeneratorId });
            modelBuilder.Entity<RegistrationIdGeneratorEntity>().Property(p => p.RegistrationId).ValueGeneratedOnAdd();
        }
    }
}
