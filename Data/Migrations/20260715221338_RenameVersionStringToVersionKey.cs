using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameVersionStringToVersionKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_scores_game_versions_version_string",
                table: "scores");

            migrationBuilder.RenameColumn(
                name: "version_string",
                table: "scores",
                newName: "version_key");

            migrationBuilder.RenameIndex(
                name: "ix_scores_version_string",
                table: "scores",
                newName: "ix_scores_version_key");

            migrationBuilder.RenameColumn(
                name: "version_string",
                table: "game_versions",
                newName: "version_key");

            migrationBuilder.AddForeignKey(
                name: "fk_scores_game_versions_version_key",
                table: "scores",
                column: "version_key",
                principalTable: "game_versions",
                principalColumn: "version_key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_scores_game_versions_version_key",
                table: "scores");

            migrationBuilder.RenameColumn(
                name: "version_key",
                table: "scores",
                newName: "version_string");

            migrationBuilder.RenameIndex(
                name: "ix_scores_version_key",
                table: "scores",
                newName: "ix_scores_version_string");

            migrationBuilder.RenameColumn(
                name: "version_key",
                table: "game_versions",
                newName: "version_string");

            migrationBuilder.AddForeignKey(
                name: "fk_scores_game_versions_version_string",
                table: "scores",
                column: "version_string",
                principalTable: "game_versions",
                principalColumn: "version_string",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
