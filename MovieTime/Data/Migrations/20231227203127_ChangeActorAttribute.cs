using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTime.Data.Migrations
{
    public partial class ChangeActorAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Actors",
                newName: "ImageMimeType");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Actors",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Actors");

            migrationBuilder.RenameColumn(
                name: "ImageMimeType",
                table: "Actors",
                newName: "Img");
        }
    }
}
