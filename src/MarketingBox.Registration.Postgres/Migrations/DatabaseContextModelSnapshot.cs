﻿// <auto-generated />
using System;
using MarketingBox.Registration.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("registration-service")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MarketingBox.Registration.Service.Domain.Models.Entities.Registration.RegistrationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AffCode")
                        .HasColumnType("text");

                    b.Property<long>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<string>("AffiliateName")
                        .HasColumnType("text");

                    b.Property<int>("ApprovedType")
                        .HasColumnType("integer");

                    b.Property<bool>("AutologinUsed")
                        .HasColumnType("boolean");

                    b.Property<long?>("BrandId")
                        .HasColumnType("bigint");

                    b.Property<long>("CampaignId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset?>("ConversionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<int>("CountryId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CrmStatus")
                        .HasColumnType("integer");

                    b.Property<string>("CustomerBrand")
                        .HasColumnType("text");

                    b.Property<string>("CustomerId")
                        .HasColumnType("text");

                    b.Property<string>("CustomerLoginUrl")
                        .HasColumnType("text");

                    b.Property<string>("CustomerToken")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("DepositDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("Funnel")
                        .HasColumnType("text");

                    b.Property<string>("Integration")
                        .HasColumnType("text");

                    b.Property<long?>("IntegrationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Sub1")
                        .HasColumnType("text");

                    b.Property<string>("Sub10")
                        .HasColumnType("text");

                    b.Property<string>("Sub2")
                        .HasColumnType("text");

                    b.Property<string>("Sub3")
                        .HasColumnType("text");

                    b.Property<string>("Sub4")
                        .HasColumnType("text");

                    b.Property<string>("Sub5")
                        .HasColumnType("text");

                    b.Property<string>("Sub6")
                        .HasColumnType("text");

                    b.Property<string>("Sub7")
                        .HasColumnType("text");

                    b.Property<string>("Sub8")
                        .HasColumnType("text");

                    b.Property<string>("Sub9")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("UniqueId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Status");

                    b.HasIndex("CreatedAt", "BrandId");

                    b.HasIndex("DepositDate", "BrandId");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("registrations", "registration-service");
                });

            modelBuilder.Entity("MarketingBox.Registration.Service.Domain.Models.Entities.Registration.RegistrationIdGeneratorEntity", b =>
                {
                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("GeneratorId")
                        .HasColumnType("text");

                    b.Property<long>("RegistrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("RegistrationId"));

                    b.HasKey("TenantId", "GeneratorId");

                    b.ToTable("registration_id_generator", "registration-service");
                });
#pragma warning restore 612, 618
        }
    }
}
