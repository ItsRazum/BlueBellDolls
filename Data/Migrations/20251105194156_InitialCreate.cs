using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueBellDolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    is_male = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "litters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    letter = table.Column<char>(type: "character(1)", nullable: false),
                    birth_day = table.Column<DateOnly>(type: "date", nullable: false),
                    mother_cat_id = table.Column<int>(type: "integer", nullable: true),
                    father_cat_id = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_litters", x => x.id);
                    table.ForeignKey(
                        name: "FK_litters_cats_father_cat_id",
                        column: x => x.father_cat_id,
                        principalTable: "cats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_litters_cats_mother_cat_id",
                        column: x => x.mother_cat_id,
                        principalTable: "cats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kittens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    @class = table.Column<string>(name: "class", type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    litter_id = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    is_male = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kittens", x => x.id);
                    table.ForeignKey(
                        name: "FK_kittens_litters_litter_id",
                        column: x => x.litter_id,
                        principalTable: "litters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    parent_cat_id = table.Column<int>(type: "integer", nullable: true),
                    kitten_id = table.Column<int>(type: "integer", nullable: true),
                    litter_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_photos_cats_parent_cat_id",
                        column: x => x.parent_cat_id,
                        principalTable: "cats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_photos_kittens_kitten_id",
                        column: x => x.kitten_id,
                        principalTable: "kittens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_photos_litters_litter_id",
                        column: x => x.litter_id,
                        principalTable: "litters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<string>(type: "text", nullable: false),
                    entity_photo_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telegram_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_telegram_photos_photos_entity_photo_id",
                        column: x => x.entity_photo_id,
                        principalTable: "photos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kittens_litter_id",
                table: "kittens",
                column: "litter_id");

            migrationBuilder.CreateIndex(
                name: "IX_litters_father_cat_id",
                table: "litters",
                column: "father_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_litters_mother_cat_id",
                table: "litters",
                column: "mother_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_photos_kitten_id",
                table: "photos",
                column: "kitten_id");

            migrationBuilder.CreateIndex(
                name: "IX_photos_litter_id",
                table: "photos",
                column: "litter_id");

            migrationBuilder.CreateIndex(
                name: "IX_photos_parent_cat_id",
                table: "photos",
                column: "parent_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_telegram_photos_entity_photo_id",
                table: "telegram_photos",
                column: "entity_photo_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_photos");

            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.DropTable(
                name: "kittens");

            migrationBuilder.DropTable(
                name: "litters");

            migrationBuilder.DropTable(
                name: "cats");
        }
    }
}
