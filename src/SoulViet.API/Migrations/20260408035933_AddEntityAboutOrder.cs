using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityAboutOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                schema: "marketplace",
                table: "Orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                schema: "marketplace",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryOrServiceDate",
                schema: "marketplace",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                schema: "marketplace",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingTrackingCode",
                schema: "marketplace",
                table: "Orders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShopDiscountAmount",
                schema: "marketplace",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShopVoucherCode",
                schema: "marketplace",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformDiscountAmount",
                schema: "marketplace",
                table: "MasterOrders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PlatformVoucherCode",
                schema: "marketplace",
                table: "MasterOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalItemsPrice",
                schema: "marketplace",
                table: "MasterOrders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalShippingFee",
                schema: "marketplace",
                table: "MasterOrders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Vouchers",
                schema: "marketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UsageLimit = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UsedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                schema: "marketplace",
                table: "Vouchers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PartnerId",
                schema: "marketplace",
                table: "Vouchers",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vouchers",
                schema: "marketplace");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryOrServiceDate",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingTrackingCode",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopDiscountAmount",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopVoucherCode",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PlatformDiscountAmount",
                schema: "marketplace",
                table: "MasterOrders");

            migrationBuilder.DropColumn(
                name: "PlatformVoucherCode",
                schema: "marketplace",
                table: "MasterOrders");

            migrationBuilder.DropColumn(
                name: "TotalItemsPrice",
                schema: "marketplace",
                table: "MasterOrders");

            migrationBuilder.DropColumn(
                name: "TotalShippingFee",
                schema: "marketplace",
                table: "MasterOrders");
        }
    }
}
