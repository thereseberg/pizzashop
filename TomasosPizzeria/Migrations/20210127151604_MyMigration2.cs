using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations
{
    public partial class MyMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gatuadress",
                table: "AspNetUsers",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Namn",
                table: "AspNetUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postnr",
                table: "AspNetUsers",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postort",
                table: "AspNetUsers",
                type: "varchar(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gatuadress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Namn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Postnr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Postort",
                table: "AspNetUsers");
        }
    }
}
