using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulViet.API.Migrations
{
    /// <inheritdoc />
    public partial class addConversationandMessageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserBId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastReadMessageIdA = table.Column<Guid>(type: "uuid", nullable: true),
                    LastReadMessageIdB = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.CheckConstraint("CK_Conversation_UserOrder", "\"UserAId\" < \"UserBId\"");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    ClientTempId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "social",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastReadMessageIdA",
                schema: "social",
                table: "Conversations",
                column: "LastReadMessageIdA");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastReadMessageIdB",
                schema: "social",
                table: "Conversations",
                column: "LastReadMessageIdB");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserAId_UserBId",
                schema: "social",
                table: "Conversations",
                columns: new[] { "UserAId", "UserBId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_ClientTempId",
                schema: "social",
                table: "Messages",
                columns: new[] { "ConversationId", "ClientTempId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_CreatedAt",
                schema: "social",
                table: "Messages",
                columns: new[] { "ConversationId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Messages_LastReadMessageIdA",
                schema: "social",
                table: "Conversations",
                column: "LastReadMessageIdA",
                principalSchema: "social",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Messages_LastReadMessageIdB",
                schema: "social",
                table: "Conversations",
                column: "LastReadMessageIdB",
                principalSchema: "social",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Messages_LastReadMessageIdA",
                schema: "social",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Messages_LastReadMessageIdB",
                schema: "social",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "social");

            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "social");
        }
    }
}
