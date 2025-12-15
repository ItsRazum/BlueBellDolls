using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueBellDolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatColors_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatColorId",
                table: "photos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatColors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatColors", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_photos_CatColorId",
                table: "photos",
                column: "CatColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_CatColors_CatColorId",
                table: "photos",
                column: "CatColorId",
                principalTable: "CatColors",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_CatColors_CatColorId",
                table: "photos");

            migrationBuilder.DropTable(
                name: "CatColors");

            migrationBuilder.DropIndex(
                name: "IX_photos_CatColorId",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "CatColorId",
                table: "photos");
        }
    }
}
