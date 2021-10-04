﻿using MarketingBox.Registration.Postgres.Entities.Deposit;
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

        private const string DepositTableName = "deposits";

        public DbSet<DepositEntity> Deposits { get; set; }


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

            SetPartnerEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetPartnerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeadEntity>().ToTable(LeadTableName);
            modelBuilder.Entity<LeadEntity>().HasKey(e => e.LeadId);
            modelBuilder.Entity<LeadEntity>().OwnsOne(x => x.BrandRegistrationInfo);   //modelBuilder.Entity<LeadEntity>().Ignore(x => x.GeneralInfo);
            modelBuilder.Entity<LeadEntity>().OwnsOne(x => x.AdditionalInfo);
            modelBuilder.Entity<LeadEntity>().HasIndex(e => new {e.TenantId, e.LeadId});

            modelBuilder.Entity<DepositEntity>().ToTable(DepositTableName);
            modelBuilder.Entity<DepositEntity>().HasKey(e => e.DepositId);
            modelBuilder.Entity<DepositEntity>().HasIndex(e => new { e.TenantId, e.DepositId});
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
