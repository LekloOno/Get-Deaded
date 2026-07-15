using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModesAndVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores");

            migrationBuilder.AddColumn<string>(
                name: "mode_key",
                table: "scores",
                type: "character varying(64)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "version_string",
                table: "scores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "game_modes",
                columns: table => new
                {
                    mode_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_modes", x => x.mode_key);
                });

            migrationBuilder.CreateTable(
                name: "score_compatibility_groups",
                columns: table => new
                {
                    group_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_score_compatibility_groups", x => x.group_key);
                });

            migrationBuilder.CreateTable(
                name: "game_versions",
                columns: table => new
                {
                    version_string = table.Column<string>(type: "text", nullable: false),
                    group_key = table.Column<string>(type: "character varying(64)", nullable: false),
                    released_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_versions", x => x.version_string);
                    table.ForeignKey(
                        name: "fk_game_versions_score_compatibility_groups_group_key",
                        column: x => x.group_key,
                        principalTable: "score_compatibility_groups",
                        principalColumn: "group_key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "game_modes",
                column: "mode_key",
                value: "test");

            migrationBuilder.InsertData(
                table: "score_compatibility_groups",
                column: "group_key",
                values: new object[]
                {
                    "0.3_sectors",
                    "pre-0.3"
                });

            migrationBuilder.InsertData(
                table: "game_versions",
                columns: new[] { "version_string", "group_key", "released_at" },
                values: new object[,]
                {
                    { "0.2.4", "pre-0.3", new DateTime(2026, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "0.3.0", "0.3_sectors", new DateTime(2026, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores",
                columns: new[] { "map_key", "mode_key", "difficulty", "player_id", "value" },
                descending: new[] { false, false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "ix_scores_mode_key",
                table: "scores",
                column: "mode_key");

            migrationBuilder.CreateIndex(
                name: "ix_scores_version_string",
                table: "scores",
                column: "version_string");

            migrationBuilder.CreateIndex(
                name: "ix_game_versions_group_key",
                table: "game_versions",
                column: "group_key");

            migrationBuilder.AddForeignKey(
                name: "fk_scores_game_modes_mode_key",
                table: "scores",
                column: "mode_key",
                principalTable: "game_modes",
                principalColumn: "mode_key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_scores_game_versions_version_string",
                table: "scores",
                column: "version_string",
                principalTable: "game_versions",
                principalColumn: "version_string",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_scores_game_modes_mode_key",
                table: "scores");

            migrationBuilder.DropForeignKey(
                name: "fk_scores_game_versions_version_string",
                table: "scores");

            migrationBuilder.DropTable(
                name: "game_modes");

            migrationBuilder.DropTable(
                name: "game_versions");

            migrationBuilder.DropTable(
                name: "score_compatibility_groups");

            migrationBuilder.DropIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores");

            migrationBuilder.DropIndex(
                name: "ix_scores_mode_key",
                table: "scores");

            migrationBuilder.DropIndex(
                name: "ix_scores_version_string",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "mode_key",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "version_string",
                table: "scores");

            migrationBuilder.CreateIndex(
                name: "ix_scores_leaderboard_lookup",
                table: "scores",
                columns: new[] { "map_key", "difficulty", "player_id", "value" },
                descending: new[] { false, false, false, true });
        }
    }
}
