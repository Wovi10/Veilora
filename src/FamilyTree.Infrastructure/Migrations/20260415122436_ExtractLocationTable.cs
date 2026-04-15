using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtractLocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Entities_BirthPlaceEntityId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Entities_DeathPlaceEntityId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "EntityLocations");

            migrationBuilder.RenameColumn(
                name: "DeathPlaceEntityId",
                table: "Characters",
                newName: "DeathPlaceLocationId");

            migrationBuilder.RenameColumn(
                name: "BirthPlaceEntityId",
                table: "Characters",
                newName: "BirthPlaceLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_DeathPlaceEntityId",
                table: "Characters",
                newName: "IX_Characters_DeathPlaceLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_BirthPlaceEntityId",
                table: "Characters",
                newName: "IX_Characters_BirthPlaceLocationId");

            migrationBuilder.CreateTable(
                name: "Locations",
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
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLocations",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLocations", x => new { x.CharacterId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_CharacterLocations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_CharacterId",
                table: "CharacterLocations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_LocationId",
                table: "CharacterLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_WorldId",
                table: "Locations",
                column: "WorldId");

            // Clear stale Entity IDs — old Place entities no longer exist in Locations table
            migrationBuilder.Sql("UPDATE \"Characters\" SET \"BirthPlaceLocationId\" = NULL, \"DeathPlaceLocationId\" = NULL;");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Locations_BirthPlaceLocationId",
                table: "Characters",
                column: "BirthPlaceLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Locations_DeathPlaceLocationId",
                table: "Characters",
                column: "DeathPlaceLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Locations_BirthPlaceLocationId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Locations_DeathPlaceLocationId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterLocations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.RenameColumn(
                name: "DeathPlaceLocationId",
                table: "Characters",
                newName: "DeathPlaceEntityId");

            migrationBuilder.RenameColumn(
                name: "BirthPlaceLocationId",
                table: "Characters",
                newName: "BirthPlaceEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_DeathPlaceLocationId",
                table: "Characters",
                newName: "IX_Characters_DeathPlaceEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_BirthPlaceLocationId",
                table: "Characters",
                newName: "IX_Characters_BirthPlaceEntityId");

            migrationBuilder.CreateTable(
                name: "EntityLocations",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLocations", x => new { x.CharacterId, x.PlaceId });
                    table.ForeignKey(
                        name: "FK_EntityLocations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityLocations_Entities_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityLocations_CharacterId",
                table: "EntityLocations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLocations_PlaceId",
                table: "EntityLocations",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Entities_BirthPlaceEntityId",
                table: "Characters",
                column: "BirthPlaceEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Entities_DeathPlaceEntityId",
                table: "Characters",
                column: "DeathPlaceEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
