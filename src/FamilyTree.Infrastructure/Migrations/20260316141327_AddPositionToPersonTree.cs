using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToPersonTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PositionX",
                table: "PersonTrees",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PositionY",
                table: "PersonTrees",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "PersonTrees");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "PersonTrees");
        }
    }
}
