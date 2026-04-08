using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldInfoProvinceInMarketProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProvinceId",
                schema: "marketplace",
                table: "MarketProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                schema: "marketplace",
                table: "MarketProducts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvinceId",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                schema: "marketplace",
                table: "MarketProducts");
        }
    }
}
