using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class statuschangelog_Add_TenantId_UserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "registration-service",
                table: "status-change-log",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "registration-service",
                table: "status-change-log",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "registration-service",
                table: "status-change-log");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "registration-service",
                table: "status-change-log");
        }
    }
}
