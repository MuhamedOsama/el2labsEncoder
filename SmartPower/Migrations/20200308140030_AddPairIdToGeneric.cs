using Microsoft.EntityFrameworkCore.Migrations;

namespace Encoder.Migrations
{
    public partial class AddPairIdToGeneric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PairId",
                table: "GenericReadings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PairId",
                table: "GenericReadings");
        }
    }
}
