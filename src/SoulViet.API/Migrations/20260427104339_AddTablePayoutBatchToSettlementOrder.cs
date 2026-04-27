using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTablePayoutBatchToSettlementOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                schema: "marketplace",
                table: "OrderItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayoutBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalSales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetPayout = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutBatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems",
                column: "PayoutBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_PayoutBatches_PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems",
                column: "PayoutBatchId",
                principalTable: "PayoutBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_PayoutBatches_PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "PayoutBatches");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsSettled",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PayoutBatchId",
                schema: "marketplace",
                table: "OrderItems");
        }
    }
}
