using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThoughtsApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensAndImprovedKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Id",
                schema: "thoughtsApp",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Thoughts_Id",
                schema: "thoughtsApp",
                table: "Thoughts");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Id",
                schema: "thoughtsApp",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_Id",
                schema: "thoughtsApp",
                table: "Reactions");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "thoughtsApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "thoughtsApp",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "thoughtsApp",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "thoughtsApp");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                schema: "thoughtsApp",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thoughts_Id",
                schema: "thoughtsApp",
                table: "Thoughts",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Id",
                schema: "thoughtsApp",
                table: "Roles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_Id",
                schema: "thoughtsApp",
                table: "Reactions",
                column: "Id",
                unique: true);
        }
    }
}
