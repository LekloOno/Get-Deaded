using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maps",
                columns: table => new
                {
                    MapKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maps", x => x.MapKey);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    WeaponKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.WeaponKey);
                });

            migrationBuilder.CreateTable(
                name: "scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MapKey = table.Column<string>(type: "character varying(64)", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    TimeMs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scores_maps_MapKey",
                        column: x => x.MapKey,
                        principalTable: "maps",
                        principalColumn: "MapKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scores_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeaponKey = table.Column<string>(type: "character varying(64)", nullable: false),
                    ScoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Damage = table.Column<int>(type: "integer", nullable: false),
                    Kills = table.Column<int>(type: "integer", nullable: false),
                    Accuracy = table.Column<float>(type: "real", nullable: false),
                    CriticalAccuracy = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weapon_stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_weapon_stats_Weapons_WeaponKey",
                        column: x => x.WeaponKey,
                        principalTable: "Weapons",
                        principalColumn: "WeaponKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_weapon_stats_scores_ScoreId",
                        column: x => x.ScoreId,
                        principalTable: "scores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_players_Username",
                table: "players",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scores_MapKey",
                table: "scores",
                column: "MapKey");

            migrationBuilder.CreateIndex(
                name: "IX_scores_PlayerId",
                table: "scores",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_weapon_stats_ScoreId",
                table: "weapon_stats",
                column: "ScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_weapon_stats_WeaponKey",
                table: "weapon_stats",
                column: "WeaponKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weapon_stats");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "scores");

            migrationBuilder.DropTable(
                name: "maps");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
