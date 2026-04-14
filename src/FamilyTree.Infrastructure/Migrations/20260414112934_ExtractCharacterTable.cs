using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtractCharacterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old FKs first
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_BirthPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_DeathPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_Parent1Id",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_Parent2Id",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityAffiliations_Entities_CharacterId",
                table: "EntityAffiliations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityLanguages_Entities_CharacterId",
                table: "EntityLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityLocations_Entities_CharacterId",
                table: "EntityLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entities_Entity1Id",
                table: "Relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entities_Entity2Id",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Entities_BirthPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_DeathPlaceEntityId",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_Parent1Id",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_Parent2Id",
                table: "Entities");

            migrationBuilder.RenameColumn(
                name: "Entity2Id",
                table: "Relationships",
                newName: "Character2Id");

            migrationBuilder.RenameColumn(
                name: "Entity1Id",
                table: "Relationships",
                newName: "Character1Id");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_Entity2Id",
                table: "Relationships",
                newName: "IX_Relationships_Character2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_Entity1Id",
                table: "Relationships",
                newName: "IX_Relationships_Character1Id");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MaidenName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Species = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthDateSuffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DeathDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DeathDateSuffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BirthPlaceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeathPlaceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Residence = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    ProfilePhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OtherNames = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Position = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Height = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HairColour = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Parent1Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Parent2Id = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_Parent1Id",
                        column: x => x.Parent1Id,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_Parent2Id",
                        column: x => x.Parent2Id,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Entities_BirthPlaceEntityId",
                        column: x => x.BirthPlaceEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Entities_DeathPlaceEntityId",
                        column: x => x.DeathPlaceEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterFamilyTrees",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FamilyTreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: true),
                    PositionY = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterFamilyTrees", x => new { x.CharacterId, x.FamilyTreeId });
                    table.ForeignKey(
                        name: "FK_CharacterFamilyTrees_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterFamilyTrees_FamilyTrees_FamilyTreeId",
                        column: x => x.FamilyTreeId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFamilyTrees_CharacterId",
                table: "CharacterFamilyTrees",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFamilyTrees_FamilyTreeId",
                table: "CharacterFamilyTrees",
                column: "FamilyTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BirthPlaceEntityId",
                table: "Characters",
                column: "BirthPlaceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_DeathPlaceEntityId",
                table: "Characters",
                column: "DeathPlaceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Parent1Id",
                table: "Characters",
                column: "Parent1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Parent2Id",
                table: "Characters",
                column: "Parent2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId",
                table: "Characters",
                column: "WorldId");

            // --- Data migration: copy character rows from Entities to Characters ---
            // Insert without self-referential parents first to avoid FK ordering issues
            migrationBuilder.Sql(@"
                INSERT INTO ""Characters"" (""Id"", ""Name"", ""WorldId"", ""Description"",
                    ""FirstName"", ""LastName"", ""MiddleName"", ""MaidenName"", ""Species"", ""Gender"",
                    ""BirthDate"", ""BirthDateSuffix"", ""DeathDate"", ""DeathDateSuffix"",
                    ""BirthPlaceEntityId"", ""DeathPlaceEntityId"", ""Residence"", ""Biography"",
                    ""ProfilePhotoUrl"", ""OtherNames"", ""Position"", ""Height"", ""HairColour"",
                    ""Parent1Id"", ""Parent2Id"", ""CreatedAt"", ""UpdatedAt"")
                SELECT ""Id"", ""Name"", ""WorldId"", ""Description"",
                    ""FirstName"", ""LastName"", ""MiddleName"", ""MaidenName"", ""Species"", ""Gender"",
                    ""BirthDate"", ""BirthDateSuffix"", ""DeathDate"", ""DeathDateSuffix"",
                    NULL, NULL, ""Residence"", ""Biography"",
                    ""ProfilePhotoUrl"", ""OtherNames"", ""Position"", ""Height"", ""HairColour"",
                    NULL, NULL, ""CreatedAt"", ""UpdatedAt""
                FROM ""Entities"" WHERE ""Type"" = 'Character'
            ");

            // Update parent references now that all characters exist
            migrationBuilder.Sql(@"
                UPDATE ""Characters"" c
                SET ""Parent1Id"" = e.""Parent1Id"", ""Parent2Id"" = e.""Parent2Id""
                FROM ""Entities"" e
                WHERE c.""Id"" = e.""Id"" AND e.""Type"" = 'Character'
            ");

            // Update BirthPlace/DeathPlace references (these reference Place entities which stay in Entities)
            migrationBuilder.Sql(@"
                UPDATE ""Characters"" c
                SET ""BirthPlaceEntityId"" = e.""BirthPlaceEntityId"",
                    ""DeathPlaceEntityId"" = e.""DeathPlaceEntityId""
                FROM ""Entities"" e
                WHERE c.""Id"" = e.""Id"" AND e.""Type"" = 'Character'
            ");

            // Copy EntityFamilyTrees → CharacterFamilyTrees
            migrationBuilder.Sql(@"
                INSERT INTO ""CharacterFamilyTrees"" (""CharacterId"", ""FamilyTreeId"", ""PositionX"", ""PositionY"")
                SELECT ""EntityId"", ""FamilyTreeId"", ""PositionX"", ""PositionY""
                FROM ""EntityFamilyTrees""
                WHERE ""EntityId"" IN (SELECT ""Id"" FROM ""Characters"")
            ");

            // Drop EntityFamilyTrees now that data is migrated
            migrationBuilder.DropTable(name: "EntityFamilyTrees");

            // Drop character-specific columns from Entities
            migrationBuilder.DropColumn(name: "Biography", table: "Entities");
            migrationBuilder.DropColumn(name: "BirthDate", table: "Entities");
            migrationBuilder.DropColumn(name: "BirthDateSuffix", table: "Entities");
            migrationBuilder.DropColumn(name: "BirthPlaceEntityId", table: "Entities");
            migrationBuilder.DropColumn(name: "DeathDate", table: "Entities");
            migrationBuilder.DropColumn(name: "DeathDateSuffix", table: "Entities");
            migrationBuilder.DropColumn(name: "DeathPlaceEntityId", table: "Entities");
            migrationBuilder.DropColumn(name: "FirstName", table: "Entities");
            migrationBuilder.DropColumn(name: "Gender", table: "Entities");
            migrationBuilder.DropColumn(name: "HairColour", table: "Entities");
            migrationBuilder.DropColumn(name: "Height", table: "Entities");
            migrationBuilder.DropColumn(name: "LastName", table: "Entities");
            migrationBuilder.DropColumn(name: "MaidenName", table: "Entities");
            migrationBuilder.DropColumn(name: "MiddleName", table: "Entities");
            migrationBuilder.DropColumn(name: "OtherNames", table: "Entities");
            migrationBuilder.DropColumn(name: "Parent1Id", table: "Entities");
            migrationBuilder.DropColumn(name: "Parent2Id", table: "Entities");
            migrationBuilder.DropColumn(name: "Position", table: "Entities");
            migrationBuilder.DropColumn(name: "ProfilePhotoUrl", table: "Entities");
            migrationBuilder.DropColumn(name: "Residence", table: "Entities");
            migrationBuilder.DropColumn(name: "Species", table: "Entities");

            // Delete character rows from Entities now that they live in Characters
            migrationBuilder.Sql(@"DELETE FROM ""Entities"" WHERE ""Type"" = 'Character'");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityAffiliations_Characters_CharacterId",
                table: "EntityAffiliations",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityLanguages_Characters_CharacterId",
                table: "EntityLanguages",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityLocations_Characters_CharacterId",
                table: "EntityLocations",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Characters_Character1Id",
                table: "Relationships",
                column: "Character1Id",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Characters_Character2Id",
                table: "Relationships",
                column: "Character2Id",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityAffiliations_Characters_CharacterId",
                table: "EntityAffiliations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityLanguages_Characters_CharacterId",
                table: "EntityLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityLocations_Characters_CharacterId",
                table: "EntityLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Characters_Character1Id",
                table: "Relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Characters_Character2Id",
                table: "Relationships");

            migrationBuilder.DropTable(
                name: "CharacterFamilyTrees");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.RenameColumn(
                name: "Character2Id",
                table: "Relationships",
                newName: "Entity2Id");

            migrationBuilder.RenameColumn(
                name: "Character1Id",
                table: "Relationships",
                newName: "Entity1Id");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_Character2Id",
                table: "Relationships",
                newName: "IX_Relationships_Entity2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_Character1Id",
                table: "Relationships",
                newName: "IX_Relationships_Entity1Id");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Entities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Entities",
                type: "date",
                nullable: true);

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

            migrationBuilder.AddColumn<DateOnly>(
                name: "DeathDate",
                table: "Entities",
                type: "date",
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
                name: "FirstName",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Entities",
                type: "character varying(20)",
                maxLength: 20,
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
                name: "LastName",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaidenName",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
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

            migrationBuilder.AddColumn<Guid>(
                name: "Parent1Id",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Parent2Id",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Entities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                table: "Entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Residence",
                table: "Entities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityFamilyTrees",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FamilyTreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: true),
                    PositionY = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFamilyTrees", x => new { x.EntityId, x.FamilyTreeId });
                    table.ForeignKey(
                        name: "FK_EntityFamilyTrees_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityFamilyTrees_FamilyTrees_FamilyTreeId",
                        column: x => x.FamilyTreeId,
                        principalTable: "FamilyTrees",
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
                name: "IX_Entities_Parent1Id",
                table: "Entities",
                column: "Parent1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_Parent2Id",
                table: "Entities",
                column: "Parent2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFamilyTrees_EntityId",
                table: "EntityFamilyTrees",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFamilyTrees_FamilyTreeId",
                table: "EntityFamilyTrees",
                column: "FamilyTreeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_Parent1Id",
                table: "Entities",
                column: "Parent1Id",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_Parent2Id",
                table: "Entities",
                column: "Parent2Id",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityAffiliations_Entities_CharacterId",
                table: "EntityAffiliations",
                column: "CharacterId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityLanguages_Entities_CharacterId",
                table: "EntityLanguages",
                column: "CharacterId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityLocations_Entities_CharacterId",
                table: "EntityLocations",
                column: "CharacterId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Entities_Entity1Id",
                table: "Relationships",
                column: "Entity1Id",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Entities_Entity2Id",
                table: "Relationships",
                column: "Entity2Id",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
