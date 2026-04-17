using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdToWorld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Worlds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CreatedById",
                table: "Worlds",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_Users_CreatedById",
                table: "Worlds",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_Users_CreatedById",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_CreatedById",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Worlds");
        }
    }
}
