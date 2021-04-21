using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations
{
    public partial class MyMigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "AspNetUsers");
                
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }
    }
}
