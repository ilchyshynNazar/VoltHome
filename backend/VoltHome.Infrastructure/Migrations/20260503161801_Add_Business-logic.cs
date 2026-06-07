using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltHome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Businesslogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "SolarHourlyCoefficients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    Coefficient = table.Column<double>(type: "double precision", precision: 5, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarHourlyCoefficients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarMonthlyCoefficients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Coefficient = table.Column<double>(type: "double precision", precision: 5, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarMonthlyCoefficients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarRegions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IrradiationKwhPerM2Year = table.Column<double>(type: "double precision", nullable: false),
                    GenerationPerKwYear = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SolarRegionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarStations_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolarStations_SolarRegions_SolarRegionId",
                        column: x => x.SolarRegionId,
                        principalTable: "SolarRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inverters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolarStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Power = table.Column<double>(type: "double precision", nullable: false),
                    Efficiency = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inverters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inverters_SolarStations_SolarStationId",
                        column: x => x.SolarStationId,
                        principalTable: "SolarStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PanelGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolarStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PanelCount = table.Column<int>(type: "integer", nullable: false),
                    PowerPerPanel = table.Column<double>(type: "double precision", nullable: false),
                    TiltAngle = table.Column<double>(type: "double precision", nullable: false),
                    AzimuthAngle = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PanelGroups_SolarStations_SolarStationId",
                        column: x => x.SolarStationId,
                        principalTable: "SolarStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolarStationEnergySnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolarStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeneratedKwhThisHour = table.Column<double>(type: "double precision", nullable: false),
                    GeneratedKwhToday = table.Column<double>(type: "double precision", nullable: true),
                    GeneratedKwhMonth = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarStationEnergySnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarStationEnergySnapshots_SolarStations_SolarStationId",
                        column: x => x.SolarStationId,
                        principalTable: "SolarStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inverters_SolarStationId",
                table: "Inverters",
                column: "SolarStationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanelGroups_SolarStationId",
                table: "PanelGroups",
                column: "SolarStationId");

            migrationBuilder.CreateIndex(
                name: "IX_SolarHourlyCoefficients_Hour",
                table: "SolarHourlyCoefficients",
                column: "Hour",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarMonthlyCoefficients_Month",
                table: "SolarMonthlyCoefficients",
                column: "Month",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarRegions_Name",
                table: "SolarRegions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarStationEnergySnapshots_SolarStationId_Timestamp",
                table: "SolarStationEnergySnapshots",
                columns: new[] { "SolarStationId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SolarStations_OwnerId",
                table: "SolarStations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SolarStations_SolarRegionId",
                table: "SolarStations",
                column: "SolarRegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inverters");

            migrationBuilder.DropTable(
                name: "PanelGroups");

            migrationBuilder.DropTable(
                name: "SolarHourlyCoefficients");

            migrationBuilder.DropTable(
                name: "SolarMonthlyCoefficients");

            migrationBuilder.DropTable(
                name: "SolarStationEnergySnapshots");

            migrationBuilder.DropTable(
                name: "SolarStations");

            migrationBuilder.DropTable(
                name: "SolarRegions");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
