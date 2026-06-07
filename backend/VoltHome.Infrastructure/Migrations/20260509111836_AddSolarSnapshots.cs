using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltHome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolarSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SolarStationEnergySnapshots_SolarStationId_Timestamp",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.DropColumn(
                name: "GeneratedKwhMonth",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.DropColumn(
                name: "GeneratedKwhThisHour",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.DropColumn(
                name: "GeneratedKwhToday",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "SolarStationEnergySnapshots",
                newName: "HourStartUtc");

            migrationBuilder.AddColumn<double>(
                name: "GeneratedKwh",
                table: "SolarStationEnergySnapshots",
                type: "double precision",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SolarCoefficient",
                table: "SolarStationEnergySnapshots",
                type: "double precision",
                precision: 5,
                scale: 3,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_SolarStationEnergySnapshots_SolarStationId_HourStartUtc",
                table: "SolarStationEnergySnapshots",
                columns: new[] { "SolarStationId", "HourStartUtc" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SolarStationEnergySnapshots_SolarStationId_HourStartUtc",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.DropColumn(
                name: "GeneratedKwh",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.DropColumn(
                name: "SolarCoefficient",
                table: "SolarStationEnergySnapshots");

            migrationBuilder.RenameColumn(
                name: "HourStartUtc",
                table: "SolarStationEnergySnapshots",
                newName: "Timestamp");

            migrationBuilder.AddColumn<double>(
                name: "GeneratedKwhMonth",
                table: "SolarStationEnergySnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeneratedKwhThisHour",
                table: "SolarStationEnergySnapshots",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GeneratedKwhToday",
                table: "SolarStationEnergySnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarStationEnergySnapshots_SolarStationId_Timestamp",
                table: "SolarStationEnergySnapshots",
                columns: new[] { "SolarStationId", "Timestamp" });
        }
    }
}
