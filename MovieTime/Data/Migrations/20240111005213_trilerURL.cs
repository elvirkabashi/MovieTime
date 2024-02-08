using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTime.Data.Migrations
{
    public partial class trilerURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrilerURL",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrilerURL",
                table: "Movies");
        }
    }
}
