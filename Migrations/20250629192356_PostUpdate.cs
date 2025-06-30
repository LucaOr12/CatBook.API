using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatBook.API.Migrations
{
    /// <inheritdoc />
    public partial class PostUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Comments",
                table: "Posts",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Posts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Posts");
        }
    }
}
