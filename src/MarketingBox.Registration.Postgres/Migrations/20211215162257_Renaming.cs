using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfoSo",
                schema: "registration-service",
                table: "registrations");

            migrationBuilder.RenameColumn(
                name: "RouteInfoStatus",
                schema: "registration-service",
                table: "registrations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RouteInfoIntegrationId",
                schema: "registration-service",
                table: "registrations",
                newName: "IntegrationId");

            migrationBuilder.RenameColumn(
                name: "RouteInfoIntegration",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub9");

            migrationBuilder.RenameColumn(
                name: "RouteInfoDepositDate",
                schema: "registration-service",
                table: "registrations",
                newName: "DepositDate");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCustomerInfoToken",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub8");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCustomerInfoLoginUrl",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub7");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCustomerInfoCustomerId",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub6");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCustomerInfoBrand",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub5");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCrmStatus",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub4");

            migrationBuilder.RenameColumn(
                name: "RouteInfoConversionDate",
                schema: "registration-service",
                table: "registrations",
                newName: "ConversionDate");

            migrationBuilder.RenameColumn(
                name: "RouteInfoCampaignId",
                schema: "registration-service",
                table: "registrations",
                newName: "CampaignId");

            migrationBuilder.RenameColumn(
                name: "RouteInfoBrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "RouteInfoApprovedType",
                schema: "registration-service",
                table: "registrations",
                newName: "CrmStatus");

            migrationBuilder.RenameColumn(
                name: "RouteInfoAffiliateId",
                schema: "registration-service",
                table: "registrations",
                newName: "AffiliateId");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub9",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub3");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub8",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub2");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub7",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub10");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub6",
                schema: "registration-service",
                table: "registrations",
                newName: "Sub1");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub5",
                schema: "registration-service",
                table: "registrations",
                newName: "Integration");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub4",
                schema: "registration-service",
                table: "registrations",
                newName: "Funnel");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub3",
                schema: "registration-service",
                table: "registrations",
                newName: "CustomerToken");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub2",
                schema: "registration-service",
                table: "registrations",
                newName: "CustomerLoginUrl");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub10",
                schema: "registration-service",
                table: "registrations",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub1",
                schema: "registration-service",
                table: "registrations",
                newName: "CustomerBrand");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfoSub",
                schema: "registration-service",
                table: "registrations",
                newName: "AffCode");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_RouteInfoStatus",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_Status");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_RouteInfoDepositDate_RouteInfoBrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_DepositDate_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_CreatedAt_RouteInfoBrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_CreatedAt_BrandId");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedType",
                schema: "registration-service",
                table: "registrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedType",
                schema: "registration-service",
                table: "registrations");

            migrationBuilder.RenameColumn(
                name: "Sub9",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoIntegration");

            migrationBuilder.RenameColumn(
                name: "Sub8",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCustomerInfoToken");

            migrationBuilder.RenameColumn(
                name: "Sub7",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCustomerInfoLoginUrl");

            migrationBuilder.RenameColumn(
                name: "Sub6",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCustomerInfoCustomerId");

            migrationBuilder.RenameColumn(
                name: "Sub5",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCustomerInfoBrand");

            migrationBuilder.RenameColumn(
                name: "Sub4",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCrmStatus");

            migrationBuilder.RenameColumn(
                name: "Sub3",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub9");

            migrationBuilder.RenameColumn(
                name: "Sub2",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub8");

            migrationBuilder.RenameColumn(
                name: "Sub10",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub7");

            migrationBuilder.RenameColumn(
                name: "Sub1",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub6");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoStatus");

            migrationBuilder.RenameColumn(
                name: "IntegrationId",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoIntegrationId");

            migrationBuilder.RenameColumn(
                name: "Integration",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub5");

            migrationBuilder.RenameColumn(
                name: "Funnel",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub4");

            migrationBuilder.RenameColumn(
                name: "DepositDate",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoDepositDate");

            migrationBuilder.RenameColumn(
                name: "CustomerToken",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub3");

            migrationBuilder.RenameColumn(
                name: "CustomerLoginUrl",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub2");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub10");

            migrationBuilder.RenameColumn(
                name: "CustomerBrand",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub1");

            migrationBuilder.RenameColumn(
                name: "CrmStatus",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoApprovedType");

            migrationBuilder.RenameColumn(
                name: "ConversionDate",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoConversionDate");

            migrationBuilder.RenameColumn(
                name: "CampaignId",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoCampaignId");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoBrandId");

            migrationBuilder.RenameColumn(
                name: "AffiliateId",
                schema: "registration-service",
                table: "registrations",
                newName: "RouteInfoAffiliateId");

            migrationBuilder.RenameColumn(
                name: "AffCode",
                schema: "registration-service",
                table: "registrations",
                newName: "AdditionalInfoSub");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_Status",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_RouteInfoStatus");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_DepositDate_BrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_RouteInfoDepositDate_RouteInfoBrandId");

            migrationBuilder.RenameIndex(
                name: "IX_registrations_CreatedAt_BrandId",
                schema: "registration-service",
                table: "registrations",
                newName: "IX_registrations_CreatedAt_RouteInfoBrandId");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfoSo",
                schema: "registration-service",
                table: "registrations",
                type: "text",
                nullable: true);
        }
    }
}
