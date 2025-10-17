using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ThoughtsApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "thoughtsApp");

            migrationBuilder.CreateTable(
                name: "Reactions",
                schema: "thoughtsApp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "thoughtsApp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "thoughtsApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Thoughts",
                schema: "thoughtsApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thoughts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thoughts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "thoughtsApp",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThoughtReactions",
                schema: "thoughtsApp",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThoughtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThoughtReactions", x => new { x.UserId, x.ThoughtId });
                    table.ForeignKey(
                        name: "FK_ThoughtReactions_Reactions_ReactionId",
                        column: x => x.ReactionId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThoughtReactions_Thoughts_ThoughtId",
                        column: x => x.ThoughtId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Thoughts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThoughtReactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "thoughtsApp",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "thoughtsApp",
                table: "Reactions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Like" },
                    { 2, "Dislike" },
                    { 3, "Laugh" }
                });

            migrationBuilder.InsertData(
                schema: "thoughtsApp",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_Id",
                schema: "thoughtsApp",
                table: "Reactions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_Name",
                schema: "thoughtsApp",
                table: "Reactions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Id",
                schema: "thoughtsApp",
                table: "Roles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "thoughtsApp",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThoughtReactions_ReactionId",
                schema: "thoughtsApp",
                table: "ThoughtReactions",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ThoughtReactions_ThoughtId",
                schema: "thoughtsApp",
                table: "ThoughtReactions",
                column: "ThoughtId");

            migrationBuilder.CreateIndex(
                name: "IX_Thoughts_Id",
                schema: "thoughtsApp",
                table: "Thoughts",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thoughts_UserId",
                schema: "thoughtsApp",
                table: "Thoughts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "thoughtsApp",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "thoughtsApp",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                schema: "thoughtsApp",
                table: "Users",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThoughtReactions",
                schema: "thoughtsApp");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "thoughtsApp");

            migrationBuilder.DropTable(
                name: "Reactions",
                schema: "thoughtsApp");

            migrationBuilder.DropTable(
                name: "Thoughts",
                schema: "thoughtsApp");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "thoughtsApp");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "thoughtsApp");
        }
    }
}
