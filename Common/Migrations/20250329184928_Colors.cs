using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBellDolls.Common.Migrations
{
    /// <inheritdoc />
    public partial class Colors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Kittens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Cats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Kittens");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Cats");
        }
    }
}
