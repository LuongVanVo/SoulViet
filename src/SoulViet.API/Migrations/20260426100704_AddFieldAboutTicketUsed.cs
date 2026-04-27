using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldAboutTicketUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTicketUsed",
                schema: "marketplace",
                table: "OrderItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TicketUsedDate",
                schema: "marketplace",
                table: "OrderItems",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTicketUsed",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "TicketUsedDate",
                schema: "marketplace",
                table: "OrderItems");
        }
    }
}
