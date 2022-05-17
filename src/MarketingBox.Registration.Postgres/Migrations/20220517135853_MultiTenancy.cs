using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class MultiTenancy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_status-change-log_TenantId_Id",
                schema: "registration-service",
                table: "status-change-log",
                columns: new[] { "TenantId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_status-change-log_TenantId_Id",
                schema: "registration-service",
                table: "status-change-log");
        }
    }
}
