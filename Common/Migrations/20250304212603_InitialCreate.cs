using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBellDolls.Common.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GeneticTestOne = table.Column<string>(type: "TEXT", nullable: false),
                    GeneticTestTwo = table.Column<string>(type: "TEXT", nullable: false),
                    Titles = table.Column<string>(type: "TEXT", nullable: false),
                    OldDescription = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    BirthDay = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    IsMale = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Photos = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Litters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Letter = table.Column<char>(type: "TEXT", nullable: false),
                    BirthDay = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MotherCatId = table.Column<int>(type: "INTEGER", nullable: true),
                    FatherCatId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Photos = table.Column<string>(type: "TEXT", nullable: false),
                    ParentCatId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Litters_Cats_FatherCatId",
                        column: x => x.FatherCatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Litters_Cats_MotherCatId",
                        column: x => x.MotherCatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Litters_Cats_ParentCatId",
                        column: x => x.ParentCatId,
                        principalTable: "Cats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Kittens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Class = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    LitterId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    BirthDay = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    IsMale = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Photos = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kittens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kittens_Litters_LitterId",
                        column: x => x.LitterId,
                        principalTable: "Litters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kittens_LitterId",
                table: "Kittens",
                column: "LitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Litters_FatherCatId",
                table: "Litters",
                column: "FatherCatId");

            migrationBuilder.CreateIndex(
                name: "IX_Litters_MotherCatId",
                table: "Litters",
                column: "MotherCatId");

            migrationBuilder.CreateIndex(
                name: "IX_Litters_ParentCatId",
                table: "Litters",
                column: "ParentCatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kittens");

            migrationBuilder.DropTable(
                name: "Litters");

            migrationBuilder.DropTable(
                name: "Cats");
        }
    }
}
