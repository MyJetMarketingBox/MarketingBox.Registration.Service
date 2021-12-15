﻿using MarketingBox.Registration.Postgres.Entities.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "registration-service";

        private const string RegistrationTableName = "registrations";

        public DbSet<RegistrationEntity> Registrations { get; set; }

        private const string RegistrationIdGeneratorTableName = "registration_id_generator";

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

            SetEntities(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistrationEntity>().ToTable(RegistrationTableName);
            modelBuilder.Entity<RegistrationEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<RegistrationEntity>().HasIndex(e => new {e.TenantId, e.Id});
            modelBuilder.Entity<RegistrationEntity>().HasIndex(e => new { e.CreatedAt, e.BrandId, });
            modelBuilder.Entity<RegistrationEntity>().HasIndex(e => new { e.DepositDate, e.BrandId, });
            modelBuilder.Entity<RegistrationEntity>().HasIndex(e => new { e.Status });

            modelBuilder.Entity<RegistrationIdGeneratorEntity>().ToTable(RegistrationIdGeneratorTableName);
            modelBuilder.Entity<RegistrationIdGeneratorEntity>().HasKey(e => new { e.TenantId, e.GeneratorId });
            modelBuilder.Entity<RegistrationIdGeneratorEntity>().Property(p => p.RegistrationId).ValueGeneratedOnAdd();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
