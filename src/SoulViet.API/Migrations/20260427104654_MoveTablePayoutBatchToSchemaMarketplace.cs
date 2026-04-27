using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablePayoutBatchToSchemaMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PayoutBatches",
                newName: "PayoutBatches",
                newSchema: "marketplace");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PayoutBatches",
                schema: "marketplace",
                newName: "PayoutBatches");
        }
    }
}
