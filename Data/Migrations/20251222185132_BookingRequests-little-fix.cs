using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueBellDolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookingRequestslittlefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_CatColors_CatColorId",
                table: "photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatColors",
                table: "CatColors");

            migrationBuilder.RenameTable(
                name: "CatColors",
                newName: "cat_colors");

            migrationBuilder.RenameColumn(
                name: "Identifier",
                table: "cat_colors",
                newName: "identifier");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "cat_colors",
                newName: "description");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cat_colors",
                table: "cat_colors",
                column: "id");

            migrationBuilder.CreateTable(
                name: "booking_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_name = table.Column<string>(type: "text", nullable: false),
                    customer_phone = table.Column<string>(type: "text", nullable: false),
                    kitten_id = table.Column<int>(type: "integer", nullable: false),
                    curator_telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_requests_kittens_kitten_id",
                        column: x => x.kitten_id,
                        principalTable: "kittens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booking_requests_kitten_id",
                table: "booking_requests",
                column: "kitten_id");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_cat_colors_CatColorId",
                table: "photos",
                column: "CatColorId",
                principalTable: "cat_colors",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_cat_colors_CatColorId",
                table: "photos");

            migrationBuilder.DropTable(
                name: "booking_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cat_colors",
                table: "cat_colors");

            migrationBuilder.RenameTable(
                name: "cat_colors",
                newName: "CatColors");

            migrationBuilder.RenameColumn(
                name: "identifier",
                table: "CatColors",
                newName: "Identifier");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "CatColors",
                newName: "Description");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatColors",
                table: "CatColors",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_CatColors_CatColorId",
                table: "photos",
                column: "CatColorId",
                principalTable: "CatColors",
                principalColumn: "id");
        }
    }
}
