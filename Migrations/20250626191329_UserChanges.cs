using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatBook.API.Migrations
{
    /// <inheritdoc />
    public partial class UserChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_CatProfiles_ProfileId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_CatProfiles_ProfileId",
                table: "Users",
                column: "ProfileId",
                principalTable: "CatProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_CatProfiles_ProfileId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_CatProfiles_ProfileId",
                table: "Users",
                column: "ProfileId",
                principalTable: "CatProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
