using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateSuffixFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "DateSuffixes");

            migrationBuilder.AddColumn<long>(
                name: "AnchorYear",
                table: "DateSuffixes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsReversed",
                table: "DateSuffixes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Scale",
                table: "DateSuffixes",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnchorYear",
                table: "DateSuffixes");

            migrationBuilder.DropColumn(
                name: "IsReversed",
                table: "DateSuffixes");

            migrationBuilder.DropColumn(
                name: "Scale",
                table: "DateSuffixes");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "DateSuffixes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
