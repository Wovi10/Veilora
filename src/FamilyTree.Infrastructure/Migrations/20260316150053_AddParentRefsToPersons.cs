using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyTree.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParentRefsToPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Parent1Id",
                table: "Persons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Parent2Id",
                table: "Persons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Parent1Id",
                table: "Persons",
                column: "Parent1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Parent2Id",
                table: "Persons",
                column: "Parent2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Persons_Parent1Id",
                table: "Persons",
                column: "Parent1Id",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Persons_Parent2Id",
                table: "Persons",
                column: "Parent2Id",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Persons_Parent1Id",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Persons_Parent2Id",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_Parent1Id",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_Parent2Id",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Parent1Id",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Parent2Id",
                table: "Persons");
        }
    }
}
