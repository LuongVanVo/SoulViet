using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class addLocationNameToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckinLocationName",
                schema: "social",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckinLocationName",
                schema: "social",
                table: "Posts");
        }
    }
}
