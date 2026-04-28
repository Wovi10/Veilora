using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBackupUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BackupUserId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BackupUserId",
                table: "Users",
                column: "BackupUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BackupUserId",
                table: "Users",
                column: "BackupUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BackupUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BackupUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BackupUserId",
                table: "Users");
        }
    }
}
