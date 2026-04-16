using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDateSuffixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDateSuffix",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DeathDateSuffix",
                table: "Characters");

            migrationBuilder.AddColumn<Guid>(
                name: "BirthDateSuffixId",
                table: "Characters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeathDateSuffixId",
                table: "Characters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DateSuffixes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateSuffixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateSuffixes_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BirthDateSuffixId",
                table: "Characters",
                column: "BirthDateSuffixId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_DeathDateSuffixId",
                table: "Characters",
                column: "DeathDateSuffixId");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuffixes_WorldId",
                table: "DateSuffixes",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuffixes_WorldId_Abbreviation",
                table: "DateSuffixes",
                columns: new[] { "WorldId", "Abbreviation" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_DateSuffixes_BirthDateSuffixId",
                table: "Characters",
                column: "BirthDateSuffixId",
                principalTable: "DateSuffixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_DateSuffixes_DeathDateSuffixId",
                table: "Characters",
                column: "DeathDateSuffixId",
                principalTable: "DateSuffixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_DateSuffixes_BirthDateSuffixId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_DateSuffixes_DeathDateSuffixId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "DateSuffixes");

            migrationBuilder.DropIndex(
                name: "IX_Characters_BirthDateSuffixId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_DeathDateSuffixId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "BirthDateSuffixId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DeathDateSuffixId",
                table: "Characters");

            migrationBuilder.AddColumn<string>(
                name: "BirthDateSuffix",
                table: "Characters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeathDateSuffix",
                table: "Characters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
