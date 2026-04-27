using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldProductTypeSnapshotInOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeSnapshot",
                schema: "marketplace",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductTypeSnapshot",
                schema: "marketplace",
                table: "OrderItems");
        }
    }
}
