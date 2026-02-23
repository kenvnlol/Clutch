using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clutch.Migrations
{
    /// <inheritdoc />
    public partial class AddClipMediaAndStagingClips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoBlobUri",
                table: "Clips",
                newName: "Media_OriginalUpload_ContainerName");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "Clips",
                newName: "Media_OriginalUpload_BlobName");

            migrationBuilder.RenameColumn(
                name: "AvatarBlobUri",
                table: "Clips",
                newName: "Media_Avatar_ContainerName");

            migrationBuilder.AddColumn<string>(
                name: "Media_Avatar_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution1080p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution1080p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution144p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution144p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution240p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution240p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution360p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution360p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution480p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution480p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution720p_BlobName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Resolution720p_ContainerName",
                table: "Clips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StagingClips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlobReference_BlobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobReference_ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagingClips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagingClips_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagingClips_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StagingClips_AuthorId",
                table: "StagingClips",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StagingClips_GameId",
                table: "StagingClips",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StagingClips");

            migrationBuilder.DropColumn(
                name: "Media_Avatar_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution1080p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution1080p_ContainerName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution144p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution144p_ContainerName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution240p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution240p_ContainerName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution360p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution360p_ContainerName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution480p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution480p_ContainerName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution720p_BlobName",
                table: "Clips");

            migrationBuilder.DropColumn(
                name: "Media_Resolution720p_ContainerName",
                table: "Clips");

            migrationBuilder.RenameColumn(
                name: "Media_OriginalUpload_ContainerName",
                table: "Clips",
                newName: "VideoBlobUri");

            migrationBuilder.RenameColumn(
                name: "Media_OriginalUpload_BlobName",
                table: "Clips",
                newName: "Uri");

            migrationBuilder.RenameColumn(
                name: "Media_Avatar_ContainerName",
                table: "Clips",
                newName: "AvatarBlobUri");
        }
    }
}
