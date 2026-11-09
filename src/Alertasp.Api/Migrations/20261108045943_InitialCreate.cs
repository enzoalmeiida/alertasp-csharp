using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alertasp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FloodReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoadName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    District = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", nullable: false),
                    Longitude = table.Column<decimal>(type: "TEXT", nullable: false),
                    ObservedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloodReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FloodReports_District",
                table: "FloodReports",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_FloodReports_ObservedAt",
                table: "FloodReports",
                column: "ObservedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FloodReports_Status",
                table: "FloodReports",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FloodReports");
        }
    }
}
