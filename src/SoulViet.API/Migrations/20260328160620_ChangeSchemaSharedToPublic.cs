using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaSharedToPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "UserSessions",
                newName: "UserSessions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "TourGuideDetails",
                newName: "TourGuideDetails",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermissions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permissions",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserSessions",
                schema: "public",
                newName: "UserSessions");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "public",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "public",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "TourGuideDetails",
                schema: "public",
                newName: "TourGuideDetails");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "public",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "public",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "public",
                newName: "Permissions");
        }
    }
}
