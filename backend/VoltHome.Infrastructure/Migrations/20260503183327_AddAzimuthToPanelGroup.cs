using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltHome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAzimuthToPanelGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzimuthAngle",
                table: "PanelGroups");

            migrationBuilder.AddColumn<int>(
                name: "Azimuth",
                table: "PanelGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Azimuth",
                table: "PanelGroups");

            migrationBuilder.AddColumn<double>(
                name: "AzimuthAngle",
                table: "PanelGroups",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
