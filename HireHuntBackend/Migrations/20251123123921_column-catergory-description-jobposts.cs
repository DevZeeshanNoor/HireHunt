using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireHuntBackend.Migrations
{
    /// <inheritdoc />
    public partial class columncatergorydescriptionjobposts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "JobPosts");
        }
    }
}
