using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBellDolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatColorforeignkeysadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "kittens");

            migrationBuilder.DropColumn(
                name: "color",
                table: "cats");

            migrationBuilder.AddColumn<int>(
                name: "cat_color_id",
                table: "kittens",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cat_color_id",
                table: "cats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_kittens_cat_color_id",
                table: "kittens",
                column: "cat_color_id");

            migrationBuilder.CreateIndex(
                name: "IX_cats_cat_color_id",
                table: "cats",
                column: "cat_color_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cats_cat_colors_cat_color_id",
                table: "cats",
                column: "cat_color_id",
                principalTable: "cat_colors",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_kittens_cat_colors_cat_color_id",
                table: "kittens",
                column: "cat_color_id",
                principalTable: "cat_colors",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cats_cat_colors_cat_color_id",
                table: "cats");

            migrationBuilder.DropForeignKey(
                name: "FK_kittens_cat_colors_cat_color_id",
                table: "kittens");

            migrationBuilder.DropIndex(
                name: "IX_kittens_cat_color_id",
                table: "kittens");

            migrationBuilder.DropIndex(
                name: "IX_cats_cat_color_id",
                table: "cats");

            migrationBuilder.DropColumn(
                name: "cat_color_id",
                table: "kittens");

            migrationBuilder.DropColumn(
                name: "cat_color_id",
                table: "cats");

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "kittens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "cats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
