using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeType2Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type2",
                table: "Pokemons",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pokemons",
                keyColumn: "Type2",
                keyValue: null,
                column: "Type2",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type2",
                table: "Pokemons",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
