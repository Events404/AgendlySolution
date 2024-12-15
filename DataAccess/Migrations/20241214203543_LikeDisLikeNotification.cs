using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LikeDisLikeNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikeDislike_Events_EventId",
                table: "LikeDislike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikeDislike",
                table: "LikeDislike");

            migrationBuilder.RenameTable(
                name: "LikeDislike",
                newName: "LikeDislikes");

            migrationBuilder.RenameIndex(
                name: "IX_LikeDislike_EventId",
                table: "LikeDislikes",
                newName: "IX_LikeDislikes_EventId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LikeDislikes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikeDislikes",
                table: "LikeDislikes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeDislikes_UserId",
                table: "LikeDislikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDislikes_Events_EventId",
                table: "LikeDislikes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDislikes_Users_UserId",
                table: "LikeDislikes",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikeDislikes_Events_EventId",
                table: "LikeDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeDislikes_Users_UserId",
                table: "LikeDislikes");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikeDislikes",
                table: "LikeDislikes");

            migrationBuilder.DropIndex(
                name: "IX_LikeDislikes_UserId",
                table: "LikeDislikes");

            migrationBuilder.RenameTable(
                name: "LikeDislikes",
                newName: "LikeDislike");

            migrationBuilder.RenameIndex(
                name: "IX_LikeDislikes_EventId",
                table: "LikeDislike",
                newName: "IX_LikeDislike_EventId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LikeDislike",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikeDislike",
                table: "LikeDislike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDislike_Events_EventId",
                table: "LikeDislike",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
