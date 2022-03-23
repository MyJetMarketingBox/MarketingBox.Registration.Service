using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Add_CountryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                schema: "registration-service",
                table: "registrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "registration-service",
                table: "registrations");
        }
    }
}
