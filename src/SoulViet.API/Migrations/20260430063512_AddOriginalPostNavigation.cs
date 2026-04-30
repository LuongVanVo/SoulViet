using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalPostNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_OriginalPostId",
                schema: "social",
                table: "Posts",
                column: "OriginalPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_OriginalPostId",
                schema: "social",
                table: "Posts",
                column: "OriginalPostId",
                principalSchema: "social",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_OriginalPostId",
                schema: "social",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_OriginalPostId",
                schema: "social",
                table: "Posts");
        }
    }
}
