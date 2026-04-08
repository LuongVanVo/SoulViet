using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "social",
                table: "PostComments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                schema: "social",
                table: "PostComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_ParentCommentId",
                schema: "social",
                table: "PostComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_PostId_CreatedAt",
                schema: "social",
                table: "PostComments",
                columns: new[] { "PostId", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_PostComments_ParentCommentId",
                schema: "social",
                table: "PostComments",
                column: "ParentCommentId",
                principalSchema: "social",
                principalTable: "PostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_PostComments_ParentCommentId",
                schema: "social",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_ParentCommentId",
                schema: "social",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_PostId_CreatedAt",
                schema: "social",
                table: "PostComments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                schema: "social",
                table: "PostComments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "social",
                table: "PostComments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);
        }
    }
}
