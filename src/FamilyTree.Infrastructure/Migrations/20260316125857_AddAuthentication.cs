using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trees_Users_UserId",
                table: "Trees");

            migrationBuilder.DropIndex(
                name: "IX_Trees_UserId",
                table: "Trees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Trees");

            migrationBuilder.CreateIndex(
                name: "IX_Trees_CreatedBy",
                table: "Trees",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Trees_Users_CreatedBy",
                table: "Trees",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trees_Users_CreatedBy",
                table: "Trees");

            migrationBuilder.DropIndex(
                name: "IX_Trees_CreatedBy",
                table: "Trees");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Trees",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trees_UserId",
                table: "Trees",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trees_Users_UserId",
                table: "Trees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
