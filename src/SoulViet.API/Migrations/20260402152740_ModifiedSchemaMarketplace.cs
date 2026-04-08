using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSchemaMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MasterOrderId",
                schema: "marketplace",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                schema: "marketplace",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ItemMetadata",
                schema: "marketplace",
                table: "OrderItems",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "marketplace",
                table: "MarketProducts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "marketplace",
                table: "MarketProducts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByAdmin",
                schema: "marketplace",
                table: "MarketProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PromotionalPrice",
                schema: "marketplace",
                table: "MarketProducts",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MasterOrders",
                schema: "marketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MasterOrderId",
                schema: "marketplace",
                table: "Orders",
                column: "MasterOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PartnerId",
                schema: "marketplace",
                table: "Orders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_MarketProducts_CategoryId",
                schema: "marketplace",
                table: "MarketProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterOrders_UserId",
                schema: "marketplace",
                table: "MasterOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                column: "OrderId1",
                principalSchema: "marketplace",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MasterOrders_MasterOrderId",
                schema: "marketplace",
                table: "Orders",
                column: "MasterOrderId",
                principalSchema: "marketplace",
                principalTable: "MasterOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MasterOrders_MasterOrderId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "MasterOrders",
                schema: "marketplace");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MasterOrderId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PartnerId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_MarketProducts_CategoryId",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MasterOrderId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                schema: "marketplace",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemMetadata",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByAdmin",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropColumn(
                name: "PromotionalPrice",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                schema: "marketplace",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                schema: "marketplace",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "marketplace",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                schema: "marketplace",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
