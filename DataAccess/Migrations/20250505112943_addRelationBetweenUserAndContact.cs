using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addRelationBetweenUserAndContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ContactUs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUs_ApplicationUserId",
                table: "ContactUs",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactUs_AspNetUsers_ApplicationUserId",
                table: "ContactUs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactUs_AspNetUsers_ApplicationUserId",
                table: "ContactUs");

            migrationBuilder.DropIndex(
                name: "IX_ContactUs_ApplicationUserId",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ContactUs");
        }
    }
}
