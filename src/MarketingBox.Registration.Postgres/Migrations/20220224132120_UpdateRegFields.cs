using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class UpdateRegFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "registration-service",
                table: "registrations");

            migrationBuilder.AddColumn<bool>(
                name: "AutologinUsed",
                schema: "registration-service",
                table: "registrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutologinUsed",
                schema: "registration-service",
                table: "registrations");

            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "registration-service",
                table: "registrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
