using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a5ce151e-760f-44bb-a405-da38021917fc"), "LocalPartner" },
                    { new Guid("d462852e-f2be-4eaa-88a5-cb0088db17f8"), "Admin" },
                    { new Guid("d9224bba-bd7d-4ee5-b958-9ae075e29784"), "Tourist" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a5ce151e-760f-44bb-a405-da38021917fc"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d462852e-f2be-4eaa-88a5-cb0088db17f8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d9224bba-bd7d-4ee5-b958-9ae075e29784"));
        }
    }
}
