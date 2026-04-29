using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveLocalPartnerProfilesToPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourGuideDetails_LocalPartnerProfile_LocalPartnerProfileId",
                schema: "public",
                table: "TourGuideDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalPartnerProfile",
                table: "LocalPartnerProfile");

            migrationBuilder.RenameTable(
                name: "LocalPartnerProfile",
                newName: "LocalPartnerProfiles",
                newSchema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                schema: "public",
                table: "LocalPartnerProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAuthenticCertified",
                schema: "public",
                table: "LocalPartnerProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "LocalPartnerProfiles",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                schema: "public",
                table: "LocalPartnerProfiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalPartnerProfiles",
                schema: "public",
                table: "LocalPartnerProfiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LocalPartnerProfiles_UserId",
                schema: "public",
                table: "LocalPartnerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalPartnerProfiles_Users_UserId",
                schema: "public",
                table: "LocalPartnerProfiles",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourGuideDetails_LocalPartnerProfiles_LocalPartnerProfileId",
                schema: "public",
                table: "TourGuideDetails",
                column: "LocalPartnerProfileId",
                principalSchema: "public",
                principalTable: "LocalPartnerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalPartnerProfiles_Users_UserId",
                schema: "public",
                table: "LocalPartnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TourGuideDetails_LocalPartnerProfiles_LocalPartnerProfileId",
                schema: "public",
                table: "TourGuideDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalPartnerProfiles",
                schema: "public",
                table: "LocalPartnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_LocalPartnerProfiles_UserId",
                schema: "public",
                table: "LocalPartnerProfiles");

            migrationBuilder.RenameTable(
                name: "LocalPartnerProfiles",
                schema: "public",
                newName: "LocalPartnerProfile");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                table: "LocalPartnerProfile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAuthenticCertified",
                table: "LocalPartnerProfile",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LocalPartnerProfile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "LocalPartnerProfile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalPartnerProfile",
                table: "LocalPartnerProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TourGuideDetails_LocalPartnerProfile_LocalPartnerProfileId",
                schema: "public",
                table: "TourGuideDetails",
                column: "LocalPartnerProfileId",
                principalTable: "LocalPartnerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
