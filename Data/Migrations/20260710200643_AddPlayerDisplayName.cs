using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_weapon_stats_Weapons_WeaponKey",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weapons",
                table: "Weapons");

            migrationBuilder.DeleteData(
                table: "players",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.RenameTable(
                name: "Weapons",
                newName: "weapons");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "players",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE Players
                SET "DisplayName" = "Username"
                """);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "players",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_weapons",
                table: "weapons",
                column: "WeaponKey");

            migrationBuilder.AddForeignKey(
                name: "FK_weapon_stats_weapons_WeaponKey",
                table: "weapon_stats",
                column: "WeaponKey",
                principalTable: "weapons",
                principalColumn: "WeaponKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_weapon_stats_weapons_WeaponKey",
                table: "weapon_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_weapons",
                table: "weapons");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "players");

            migrationBuilder.RenameTable(
                name: "weapons",
                newName: "Weapons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weapons",
                table: "Weapons",
                column: "WeaponKey");

            migrationBuilder.InsertData(
                table: "players",
                columns: new[] { "Id", "PasswordHash", "Username" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "$2a$12$dWEi/DtIMdGlLsLFirWgfOzsrPK8dFjWGPsexIZE3cUhW1Yqi/DmO", "test" });

            migrationBuilder.AddForeignKey(
                name: "FK_weapon_stats_Weapons_WeaponKey",
                table: "weapon_stats",
                column: "WeaponKey",
                principalTable: "Weapons",
                principalColumn: "WeaponKey",
                onDelete: ReferentialAction.Cascade);
        }
    }
}