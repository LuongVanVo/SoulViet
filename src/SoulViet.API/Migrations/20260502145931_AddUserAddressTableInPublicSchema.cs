using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAddressTableInPublicSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ReceiverPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DetailedAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "public",
                table: "UserAddresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "public");
        }
    }
}
