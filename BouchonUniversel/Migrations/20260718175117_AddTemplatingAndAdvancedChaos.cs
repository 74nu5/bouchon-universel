using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BouchonUniversel.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplatingAndAdvancedChaos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConnectionResetProbability",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Templating",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TruncateResponseProbability",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionResetProbability",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Templating",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TruncateResponseProbability",
                table: "Services");
        }
    }
}
