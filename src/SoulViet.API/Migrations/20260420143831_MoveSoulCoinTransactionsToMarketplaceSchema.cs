using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveSoulCoinTransactionsToMarketplaceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SoulCoinTransactions",
                newName: "SoulCoinTransactions",
                newSchema: "marketplace");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SoulCoinTransactions",
                schema: "marketplace",
                newName: "SoulCoinTransactions");
        }
    }
}
