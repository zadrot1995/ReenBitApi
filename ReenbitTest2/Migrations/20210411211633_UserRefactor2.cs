using Microsoft.EntityFrameworkCore.Migrations;

namespace ReenbitTest2.Migrations
{
    public partial class UserRefactor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnection_AspNetUsers_UserId",
                table: "UserConnection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection");

            migrationBuilder.RenameTable(
                name: "UserConnection",
                newName: "UserConnections");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnection_UserId",
                table: "UserConnections",
                newName: "IX_UserConnections_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnections",
                table: "UserConnections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnections_AspNetUsers_UserId",
                table: "UserConnections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnections_AspNetUsers_UserId",
                table: "UserConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnections",
                table: "UserConnections");

            migrationBuilder.RenameTable(
                name: "UserConnections",
                newName: "UserConnection");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnections_UserId",
                table: "UserConnection",
                newName: "IX_UserConnection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnection_AspNetUsers_UserId",
                table: "UserConnection",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
