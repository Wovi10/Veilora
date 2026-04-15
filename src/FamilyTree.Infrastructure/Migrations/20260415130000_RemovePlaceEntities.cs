using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlaceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Entities\" WHERE \"Type\" = 'Place';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Place entities are permanently dropped — no rollback
        }
    }
}
