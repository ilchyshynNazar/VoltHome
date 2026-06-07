using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltHome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolarDailySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolarStationDailySnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolarStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalKwh = table.Column<double>(type: "double precision", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarStationDailySnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarStationDailySnapshots_SolarStations_SolarStationId",
                        column: x => x.SolarStationId,
                        principalTable: "SolarStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolarStationDailySnapshots_SolarStationId_Day",
                table: "SolarStationDailySnapshots",
                columns: new[] { "SolarStationId", "Day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolarStationDailySnapshots");
        }
    }
}
