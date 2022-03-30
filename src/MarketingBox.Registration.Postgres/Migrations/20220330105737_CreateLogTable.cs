using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Registration.Postgres.Migrations
{
    public partial class CreateLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "status-change-log",
                schema: "registration-service");
        }
    }
}
