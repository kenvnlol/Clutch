using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clutch.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalVideoUrl",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalVideoUrl",
                table: "Clips");
        }
    }
}
