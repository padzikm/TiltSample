using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiltDemoApi2.Migrations
{
    public partial class SecondSec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FirstData",
                table: "FirstData");

            migrationBuilder.RenameTable(
                name: "FirstData",
                newName: "SecondData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecondData",
                table: "SecondData",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SecondData",
                table: "SecondData");

            migrationBuilder.RenameTable(
                name: "SecondData",
                newName: "FirstData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FirstData",
                table: "FirstData",
                column: "Id");
        }
    }
}
