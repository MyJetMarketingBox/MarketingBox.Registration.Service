﻿// <auto-generated />
using System;
using MarketingBox.Registration.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211102122509_LeadCountIndicies")]
    partial class LeadCountIndicies
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("lead-service")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MarketingBox.Registration.Postgres.Entities.Lead.LeadEntity", b =>
                {
                    b.Property<long>("LeadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdditionalInfoSo")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub1")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub10")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub2")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub3")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub4")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub5")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub6")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub7")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub8")
                        .HasColumnType("text");

                    b.Property<string>("AdditionalInfoSub9")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<long>("RouteInfoAffiliateId")
                        .HasColumnType("bigint");

                    b.Property<int>("RouteInfoApprovedType")
                        .HasColumnType("integer");

                    b.Property<long>("RouteInfoBoxId")
                        .HasColumnType("bigint");

                    b.Property<string>("RouteInfoBrand")
                        .HasColumnType("text");

                    b.Property<long>("RouteInfoBrandId")
                        .HasColumnType("bigint");

                    b.Property<long>("RouteInfoCampaignId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset?>("RouteInfoConversionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RouteInfoCrmStatus")
                        .HasColumnType("text");

                    b.Property<string>("RouteInfoCustomerInfoBrand")
                        .HasColumnType("text");

                    b.Property<string>("RouteInfoCustomerInfoCustomerId")
                        .HasColumnType("text");

                    b.Property<string>("RouteInfoCustomerInfoLoginUrl")
                        .HasColumnType("text");

                    b.Property<string>("RouteInfoCustomerInfoToken")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("RouteInfoDepositDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RouteInfoStatus")
                        .HasColumnType("integer");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("UniqueId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("LeadId");

                    b.HasIndex("RouteInfoStatus");

                    b.HasIndex("CreatedAt", "RouteInfoCampaignId");

                    b.HasIndex("RouteInfoDepositDate", "RouteInfoCampaignId");

                    b.HasIndex("TenantId", "LeadId");

                    b.ToTable("leads");
                });

            modelBuilder.Entity("MarketingBox.Registration.Postgres.Entities.Lead.LeadIdGeneratorEntity", b =>
                {
                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("GeneratorId")
                        .HasColumnType("text");

                    b.Property<long>("LeadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("TenantId", "GeneratorId");

                    b.ToTable("leadidgenerator");
                });
#pragma warning restore 612, 618
        }
    }
}
