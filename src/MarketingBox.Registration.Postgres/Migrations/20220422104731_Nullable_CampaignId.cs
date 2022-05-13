using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class Nullable_CampaignId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CampaignId",
                schema: "registration-service",
                table: "registrations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CampaignId",
                schema: "registration-service",
                table: "registrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
