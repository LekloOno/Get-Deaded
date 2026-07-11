using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scores_maps_MapKey",
                table: "scores");

            migrationBuilder.DropForeignKey(
                name: "FK_scores_players_PlayerId",
                table: "scores");

            migrationBuilder.DropForeignKey(
                name: "FK_weapon_stats_scores_ScoreId",
                table: "weapon_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_weapon_stats_weapons_WeaponKey",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_weapons",
                table: "weapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_weapon_stats",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_scores",
                table: "scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_players",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_maps",
                table: "maps");

            migrationBuilder.RenameColumn(
                name: "WeaponKey",
                table: "weapons",
                newName: "weapon_key");

            migrationBuilder.RenameColumn(
                name: "Kills",
                table: "weapon_stats",
                newName: "kills");

            migrationBuilder.RenameColumn(
                name: "Damage",
                table: "weapon_stats",
                newName: "damage");

            migrationBuilder.RenameColumn(
                name: "Accuracy",
                table: "weapon_stats",
                newName: "accuracy");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "weapon_stats",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WeaponKey",
                table: "weapon_stats",
                newName: "weapon_key");

            migrationBuilder.RenameColumn(
                name: "ScoreId",
                table: "weapon_stats",
                newName: "score_id");

            migrationBuilder.RenameColumn(
                name: "CriticalAccuracy",
                table: "weapon_stats",
                newName: "critical_accuracy");

            migrationBuilder.RenameIndex(
                name: "IX_weapon_stats_WeaponKey",
                table: "weapon_stats",
                newName: "ix_weapon_stats_weapon_key");

            migrationBuilder.RenameIndex(
                name: "IX_weapon_stats_ScoreId",
                table: "weapon_stats",
                newName: "ix_weapon_stats_score_id");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "scores",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "scores",
                newName: "difficulty");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "scores",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TimeMs",
                table: "scores",
                newName: "time_ms");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "scores",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "MapKey",
                table: "scores",
                newName: "map_key");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "scores",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_scores_PlayerId",
                table: "scores",
                newName: "ix_scores_player_id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "players",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "players",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "players",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "players",
                newName: "display_name");

            migrationBuilder.RenameIndex(
                name: "IX_players_Username",
                table: "players",
                newName: "ix_players_username");

            migrationBuilder.RenameColumn(
                name: "MapKey",
                table: "maps",
                newName: "map_key");

            migrationBuilder.AddPrimaryKey(
                name: "pk_weapons",
                table: "weapons",
                column: "weapon_key");

            migrationBuilder.AddPrimaryKey(
                name: "pk_weapon_stats",
                table: "weapon_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_scores",
                table: "scores",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_players",
                table: "players",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_maps",
                table: "maps",
                column: "map_key");

            migrationBuilder.AddForeignKey(
                name: "fk_scores_maps_map_key",
                table: "scores",
                column: "map_key",
                principalTable: "maps",
                principalColumn: "map_key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_scores_players_player_id",
                table: "scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_weapon_stats_scores_score_id",
                table: "weapon_stats",
                column: "score_id",
                principalTable: "scores",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_weapon_stats_weapons_weapon_key",
                table: "weapon_stats",
                column: "weapon_key",
                principalTable: "weapons",
                principalColumn: "weapon_key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_scores_maps_map_key",
                table: "scores");

            migrationBuilder.DropForeignKey(
                name: "fk_scores_players_player_id",
                table: "scores");

            migrationBuilder.DropForeignKey(
                name: "fk_weapon_stats_scores_score_id",
                table: "weapon_stats");

            migrationBuilder.DropForeignKey(
                name: "fk_weapon_stats_weapons_weapon_key",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "pk_weapons",
                table: "weapons");

            migrationBuilder.DropPrimaryKey(
                name: "pk_weapon_stats",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "pk_scores",
                table: "scores");

            migrationBuilder.DropPrimaryKey(
                name: "pk_players",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_maps",
                table: "maps");

            migrationBuilder.RenameColumn(
                name: "weapon_key",
                table: "weapons",
                newName: "WeaponKey");

            migrationBuilder.RenameColumn(
                name: "kills",
                table: "weapon_stats",
                newName: "Kills");

            migrationBuilder.RenameColumn(
                name: "damage",
                table: "weapon_stats",
                newName: "Damage");

            migrationBuilder.RenameColumn(
                name: "accuracy",
                table: "weapon_stats",
                newName: "Accuracy");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "weapon_stats",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weapon_key",
                table: "weapon_stats",
                newName: "WeaponKey");

            migrationBuilder.RenameColumn(
                name: "score_id",
                table: "weapon_stats",
                newName: "ScoreId");

            migrationBuilder.RenameColumn(
                name: "critical_accuracy",
                table: "weapon_stats",
                newName: "CriticalAccuracy");

            migrationBuilder.RenameIndex(
                name: "ix_weapon_stats_weapon_key",
                table: "weapon_stats",
                newName: "IX_weapon_stats_WeaponKey");

            migrationBuilder.RenameIndex(
                name: "ix_weapon_stats_score_id",
                table: "weapon_stats",
                newName: "IX_weapon_stats_ScoreId");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "scores",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "difficulty",
                table: "scores",
                newName: "Difficulty");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "scores",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "time_ms",
                table: "scores",
                newName: "TimeMs");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "scores",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "map_key",
                table: "scores",
                newName: "MapKey");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "scores",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_scores_player_id",
                table: "scores",
                newName: "IX_scores_PlayerId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "players",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "players",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "players",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "players",
                newName: "DisplayName");

            migrationBuilder.RenameIndex(
                name: "ix_players_username",
                table: "players",
                newName: "IX_players_Username");

            migrationBuilder.RenameColumn(
                name: "map_key",
                table: "maps",
                newName: "MapKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_weapons",
                table: "weapons",
                column: "WeaponKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_weapon_stats",
                table: "weapon_stats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_scores",
                table: "scores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_players",
                table: "players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_maps",
                table: "maps",
                column: "MapKey");

            migrationBuilder.AddForeignKey(
                name: "FK_scores_maps_MapKey",
                table: "scores",
                column: "MapKey",
                principalTable: "maps",
                principalColumn: "MapKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_scores_players_PlayerId",
                table: "scores",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weapon_stats_scores_ScoreId",
                table: "weapon_stats",
                column: "ScoreId",
                principalTable: "scores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weapon_stats_weapons_WeaponKey",
                table: "weapon_stats",
                column: "WeaponKey",
                principalTable: "weapons",
                principalColumn: "WeaponKey",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
