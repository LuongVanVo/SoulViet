using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    public partial class AddEntityCategoryInMarketplace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Giữ lại phần dọn dẹp lỗi Shadow Property của OrderItem
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                schema: "marketplace",
                table: "OrderItems");

            // ĐÃ XÓA TOÀN BỘ CÁC LỆNH DROP/RENAME ĐỤNG CHẠM ĐẾN SCHEMA "soulmap" Ở ĐÂY

            // 2. Tạo bảng Categories cho Marketplace
            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "marketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    // ĐIỂM ĂN TIỀN LÀ ĐÂY: Đổi tên Constraint thành PK_Marketplace_Categories để không đụng hàng
                    table.PrimaryKey("PK_Marketplace_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                schema: "marketplace",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketProducts_Categories_CategoryId",
                schema: "marketplace",
                table: "MarketProducts",
                column: "CategoryId",
                principalSchema: "marketplace",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dọn dẹp sạch sẽ hàm Down, bỏ qua phần khôi phục lại bảng soulmap
            migrationBuilder.DropForeignKey(
                name: "FK_MarketProducts_Categories_CategoryId",
                schema: "marketplace",
                table: "MarketProducts");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "marketplace");

            // Khôi phục lại shadow property nếu lỡ rollback
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                schema: "marketplace",
                table: "OrderItems",
                column: "OrderId1",
                principalSchema: "marketplace",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}