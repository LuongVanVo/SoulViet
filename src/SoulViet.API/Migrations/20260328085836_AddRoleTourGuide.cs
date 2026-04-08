using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTourGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalPartnerProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PartnerType = table.Column<int>(type: "integer", nullable: false),
                    IsAuthenticCertified = table.Column<bool>(type: "boolean", nullable: false),
                    TaxId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalPartnerProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourGuideDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPartnerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExperienceYears = table.Column<int>(type: "integer", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AverageRating = table.Column<double>(type: "double precision", nullable: false),
                    Languages = table.Column<string>(type: "jsonb", nullable: false),
                    Specialties = table.Column<string>(type: "jsonb", nullable: false),
                    CoverageProvinces = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuideDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuideDetails_LocalPartnerProfile_LocalPartnerProfileId",
                        column: x => x.LocalPartnerProfileId,
                        principalTable: "LocalPartnerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideDetails_LocalPartnerProfileId",
                table: "TourGuideDetails",
                column: "LocalPartnerProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourGuideDetails");

            migrationBuilder.DropTable(
                name: "LocalPartnerProfile");
        }
    }
}
