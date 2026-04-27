using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForTicketCodeInOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_TicketCode",
                schema: "marketplace",
                table: "OrderItems",
                column: "TicketCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_TicketCode",
                schema: "marketplace",
                table: "OrderItems");
        }
    }
}
