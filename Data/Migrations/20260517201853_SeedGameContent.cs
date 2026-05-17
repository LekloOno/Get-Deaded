using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedGameContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Weapons",
                column: "WeaponKey",
                values: new object[]
                {
                    "g0z_brt",
                    "p3_w"
                });

            migrationBuilder.InsertData(
                table: "maps",
                column: "MapKey",
                value: "dust_pit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Weapons",
                keyColumn: "WeaponKey",
                keyValue: "g0z_brt");

            migrationBuilder.DeleteData(
                table: "Weapons",
                keyColumn: "WeaponKey",
                keyValue: "p3_w");

            migrationBuilder.DeleteData(
                table: "maps",
                keyColumn: "MapKey",
                keyValue: "dust_pit");
        }
    }
}
