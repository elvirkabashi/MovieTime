using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTime.Data.Migrations
{
    public partial class RatingUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Ratings");
        }
    }
}
