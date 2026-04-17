using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthPlace",
                table: "Entities",
                newName: "Position");

            migrationBuilder.AddColumn<string>(
                name: "BirthDateSuffix",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BirthPlaceEntityId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeathDateSuffix",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeathPlaceEntityId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HairColour",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherNames",
                table: "Entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityAffiliations",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityAffiliations", x => new { x.CharacterId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_EntityAffiliations_Entities_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityAffiliations_Entities_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_EntityLocations_Entities_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityLocations_Entities_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Languages_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityLanguages",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLanguages", x => new { x.CharacterId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_EntityLanguages_Entities_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entities_BirthPlaceEntityId",
                table: "Entities",
                column: "BirthPlaceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_DeathPlaceEntityId",
                table: "Entities",
                column: "DeathPlaceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAffiliations_CharacterId",
                table: "EntityAffiliations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAffiliations_GroupId",
                table: "EntityAffiliations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLanguages_CharacterId",
                table: "EntityLanguages",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLanguages_LanguageId",
                table: "EntityLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLocations_CharacterId",
                table: "EntityLocations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLocations_PlaceId",
                table: "EntityLocations",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId",
                table: "Languages",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Name",
                table: "Languages",
                columns: new[] { "WorldId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_BirthPlaceEntityId",
                table: "Entities",
                column: "BirthPlaceEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_DeathPlaceEntityId",
                table: "Entities",
                column: "DeathPlaceEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_BirthPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_DeathPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropTable(
                name: "EntityAffiliations");

            migrationBuilder.DropTable(
                name: "EntityLanguages");

            migrationBuilder.DropTable(
                name: "EntityLocations");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Entities_BirthPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_DeathPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "BirthDateSuffix",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "BirthPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "DeathDateSuffix",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "DeathPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "HairColour",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "OtherNames",
                table: "Entities");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Entities",
                newName: "BirthPlace");
        }
    }
}
