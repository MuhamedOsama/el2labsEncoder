using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Encoder.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericReadings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<string>(nullable: true),
                    LineId = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Length = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LengthReadings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<string>(nullable: true),
                    PairId = table.Column<string>(nullable: true),
                    LineId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    LastRequest = table.Column<DateTime>(nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Assignment = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LengthReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LengthReadingsLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<string>(nullable: true),
                    PairId = table.Column<string>(nullable: true),
                    LineId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Assignment = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LengthReadingsLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericReadings");

            migrationBuilder.DropTable(
                name: "LengthReadings");

            migrationBuilder.DropTable(
                name: "LengthReadingsLogs");
        }
    }
}
