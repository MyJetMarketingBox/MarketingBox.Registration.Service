using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Init : Migration
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
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    RouteInfoAffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoBrandId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoCampaignId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoIntegration = table.Column<string>(type: "text", nullable: true),
                    RouteInfoIntegrationId = table.Column<long>(type: "bigint", nullable: false),
                    RouteInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    RouteInfoApprovedType = table.Column<int>(type: "integer", nullable: false),
                    AdditionalInfoSo = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub1 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub2 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub3 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub4 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub5 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub6 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub7 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub8 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub9 = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfoSub10 = table.Column<string>(type: "text", nullable: true),
                    RouteInfoCustomerInfoCustomerId = table.Column<string>(type: "text", nullable: true),
                    RouteInfoCustomerInfoToken = table.Column<string>(type: "text", nullable: true),
                    RouteInfoCustomerInfoLoginUrl = table.Column<string>(type: "text", nullable: true),
                    RouteInfoCustomerInfoBrand = table.Column<string>(type: "text", nullable: true),
                    RouteInfoCrmStatus = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RouteInfoDepositDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RouteInfoConversionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_CreatedAt_RouteInfoBrandId",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "CreatedAt", "RouteInfoBrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_RouteInfoDepositDate_RouteInfoBrandId",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "RouteInfoDepositDate", "RouteInfoBrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_registrations_RouteInfoStatus",
                schema: "registration-service",
                table: "registrations",
                column: "RouteInfoStatus");

            migrationBuilder.CreateIndex(
                name: "IX_registrations_TenantId_Id",
                schema: "registration-service",
                table: "registrations",
                columns: new[] { "TenantId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_id_generator",
                schema: "registration-service");

            migrationBuilder.DropTable(
                name: "registrations",
                schema: "registration-service");
        }
    }
}
