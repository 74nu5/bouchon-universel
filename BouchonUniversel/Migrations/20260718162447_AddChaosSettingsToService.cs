using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BouchonUniversel.Migrations
{
    /// <inheritdoc />
    public partial class AddChaosSettingsToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErrorProbability",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorStatusCode",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LatencyMs",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorProbability",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ErrorStatusCode",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "LatencyMs",
                table: "Services");
        }
    }
}
