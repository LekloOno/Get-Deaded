using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScoresIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_scores_MapKey",
                table: "scores");

            migrationBuilder.CreateIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores",
                columns: new[] { "MapKey", "Difficulty", "PlayerId", "Value" },
                descending: new[] { false, false, false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores");

            migrationBuilder.CreateIndex(
                name: "IX_scores_MapKey",
                table: "scores",
                column: "MapKey");
        }
    }
}
