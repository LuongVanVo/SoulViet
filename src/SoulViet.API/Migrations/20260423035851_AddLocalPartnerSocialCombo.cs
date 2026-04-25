using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalPartnerSocialCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "TaggedProductIds",
                schema: "social",
                table: "Posts",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "SocialComboExperiences",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuideId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicePartnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PromotionalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MediaUrls = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialComboExperiences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialComboExperiences_GuideId",
                schema: "social",
                table: "SocialComboExperiences",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialComboExperiences_ServicePartnerId",
                schema: "social",
                table: "SocialComboExperiences",
                column: "ServicePartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialComboExperiences",
                schema: "social");

            migrationBuilder.DropColumn(
                name: "TaggedProductIds",
                schema: "social",
                table: "Posts");
        }
    }
}
