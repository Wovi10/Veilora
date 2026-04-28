using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtractEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_WorldId",
                table: "Events",
                column: "WorldId");

            migrationBuilder.Sql("""
                INSERT INTO "Events" ("Id", "Name", "WorldId", "Description", "CreatedAt", "UpdatedAt")
                SELECT "Id", "Name", "WorldId", "Description", "CreatedAt", "UpdatedAt"
                FROM "Entities"
                WHERE "Type" = 'Event';
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Entities" WHERE "Type" = 'Event';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "Entities" ("Id", "Name", "Type", "WorldId", "Description", "CreatedAt", "UpdatedAt")
                SELECT "Id", "Name", 'Event', "WorldId", "Description", "CreatedAt", "UpdatedAt"
                FROM "Events";
                """);

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
