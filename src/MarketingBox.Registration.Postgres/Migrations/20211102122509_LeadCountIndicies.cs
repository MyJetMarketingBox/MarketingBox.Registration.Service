using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class LeadCountIndicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_leads_CreatedAt_RouteInfoCampaignId",
                schema: "lead-service",
                table: "leads",
                columns: new[] { "CreatedAt", "RouteInfoCampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_RouteInfoDepositDate_RouteInfoCampaignId",
                schema: "lead-service",
                table: "leads",
                columns: new[] { "RouteInfoDepositDate", "RouteInfoCampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_RouteInfoStatus",
                schema: "lead-service",
                table: "leads",
                column: "RouteInfoStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_CreatedAt_RouteInfoCampaignId",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_RouteInfoDepositDate_RouteInfoCampaignId",
                schema: "lead-service",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_RouteInfoStatus",
                schema: "lead-service",
                table: "leads");
        }
    }
}
