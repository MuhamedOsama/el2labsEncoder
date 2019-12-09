using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartPower.Migrations
{
    public partial class ChangedTablesNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReadingsLogs",
                table: "ReadingsLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Readings",
                table: "Readings");

            migrationBuilder.RenameTable(
                name: "ReadingsLogs",
                newName: "LengthReadingsLogs");

            migrationBuilder.RenameTable(
                name: "Readings",
                newName: "LengthReadings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LengthReadingsLogs",
                table: "LengthReadingsLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LengthReadings",
                table: "LengthReadings",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LengthReadingsLogs",
                table: "LengthReadingsLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LengthReadings",
                table: "LengthReadings");

            migrationBuilder.RenameTable(
                name: "LengthReadingsLogs",
                newName: "ReadingsLogs");

            migrationBuilder.RenameTable(
                name: "LengthReadings",
                newName: "Readings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReadingsLogs",
                table: "ReadingsLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Readings",
                table: "Readings",
                column: "Id");
        }
    }
}
