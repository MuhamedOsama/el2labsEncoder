using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartPower.Migrations
{
    public partial class NewReadingFormatAndRemovedJobOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureReading");

            migrationBuilder.DropTable(
                name: "jobOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reading",
                table: "Reading");

            migrationBuilder.RenameTable(
                name: "Reading",
                newName: "Readings");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Readings",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "Readings",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "MachineCode",
                table: "Readings",
                newName: "PairId");

            migrationBuilder.AlterColumn<short>(
                name: "Status",
                table: "Readings",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<short>(
                name: "Assignment",
                table: "Readings",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Readings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "Readings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MachineId",
                table: "Readings",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Readings",
                table: "Readings",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Readings",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "Assignment",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "Readings");

            migrationBuilder.RenameTable(
                name: "Readings",
                newName: "Reading");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reading",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Reading",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "PairId",
                table: "Reading",
                newName: "MachineCode");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Reading",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reading",
                table: "Reading",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FutureReading",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Assignment = table.Column<short>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    LineId = table.Column<int>(nullable: false),
                    MachineId = table.Column<string>(nullable: true),
                    PairId = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureReading", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "jobOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EndDate = table.Column<DateTime>(nullable: true),
                    JobOrderId = table.Column<string>(nullable: true),
                    MachineCode = table.Column<string>(nullable: true),
                    ReadingId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TotalLength = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jobOrders_Reading_ReadingId",
                        column: x => x.ReadingId,
                        principalTable: "Reading",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobOrders_ReadingId",
                table: "jobOrders",
                column: "ReadingId");
        }
    }
}
