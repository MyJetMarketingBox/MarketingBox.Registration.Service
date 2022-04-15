using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "registration-service");

            migrationBuilder.CreateTable(
                name: "registration_id_generator",
                schema: "registration-service",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    GeneratorId = table.Column<string>(type: "text", nullable: false),
                    RegistrationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_id_generator", x => new { x.TenantId, x.GeneratorId });
                });

            migrationBuilder.CreateTable(
                name: "registrations",
                schema: "registration-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: true),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    AffiliateName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedType = table.Column<int>(type: "integer", nullable: false),
                    CrmStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepositDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConversionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Integration = table.Column<string>(type: "text", nullable: true),
                    IntegrationId = table.Column<long>(type: "bigint", nullable: true),
                    Funnel = table.Column<string>(type: "text", nullable: true),
                    AffCode = table.Column<string>(type: "text", nullable: true),
                    Sub1 = table.Column<string>(type: "text", nullable: true),
                    Sub2 = table.Column<string>(type: "text", nullable: true),
                    Sub3 = table.Column<string>(type: "text", nullable: true),
                    Sub4 = table.Column<string>(type: "text", nullable: true),
                    Sub5 = table.Column<string>(type: "text", nullable: true),
                    Sub6 = table.Column<string>(type: "text", nullable: true),
                    Sub7 = table.Column<string>(type: "text", nullable: true),
                    Sub8 = table.Column<string>(type: "text", nullable: true),
                    Sub9 = table.Column<string>(type: "text", nullable: true),
                    Sub10 = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    CustomerToken = table.Column<string>(type: "text", nullable: true),
                    CustomerLoginUrl = table.Column<string>(type: "text", nullable: true),
                    CustomerBrand = table.Column<string>(type: "text", nullable: true),
                    AutologinUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "status-change-log",
                schema: "registration-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RegistrationId = table.Column<long>(type: "bigint", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    OldStatus = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status-change-log", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_CreatedAt_BrandId",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "CreatedAt", "BrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_DepositDate_BrandId",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "DepositDate", "BrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_Status",
                schema: "registration-service",
                table: "registrations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_registrations_TenantId_Email_BrandId",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "TenantId", "Email", "BrandId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registrations_TenantId_Id",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_status-change-log_Mode",
                schema: "registration-service",
                table: "status-change-log",
                column: "Mode");

            migrationBuilder.CreateIndex(
                name: "IX_status-change-log_RegistrationId",
                schema: "registration-service",
                table: "status-change-log",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_status-change-log_UserId",
                schema: "registration-service",
                table: "status-change-log",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_id_generator",
                schema: "registration-service");

            migrationBuilder.DropTable(
                name: "registrations",
                schema: "registration-service");

            migrationBuilder.DropTable(
                name: "status-change-log",
                schema: "registration-service");
        }
    }
}
