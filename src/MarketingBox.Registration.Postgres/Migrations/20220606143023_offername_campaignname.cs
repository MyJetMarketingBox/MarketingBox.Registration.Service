using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class offername_campaignname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampaignName",
                schema: "registration-service",
                table: "registrations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfferName",
                schema: "registration-service",
                table: "registrations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignName",
                schema: "registration-service",
                table: "registrations");

            migrationBuilder.DropColumn(
                name: "OfferName",
                schema: "registration-service",
                table: "registrations");
        }
    }
}
