using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartPower.Migrations
{
    public partial class RemovedConsumedPropertyAndMachineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobOrders_machines_MachineId",
                table: "jobOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reading_jobOrders_JobOrderId",
                table: "Reading");

            migrationBuilder.DropTable(
                name: "machines");

            migrationBuilder.DropIndex(
                name: "IX_Reading_JobOrderId",
                table: "Reading");

            migrationBuilder.DropColumn(
                name: "JobOrderId",
                table: "Reading");

            migrationBuilder.DropColumn(
                name: "Consumed",
                table: "jobOrders");

            migrationBuilder.RenameColumn(
                name: "MachineId",
                table: "jobOrders",
                newName: "JobOrder");

            migrationBuilder.RenameIndex(
                name: "IX_jobOrders_MachineId",
                table: "jobOrders",
                newName: "IX_jobOrders_JobOrder");

            migrationBuilder.AlterColumn<string>(
                name: "JobOrderId",
                table: "jobOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_jobOrders_Reading_JobOrder",
                table: "jobOrders",
                column: "JobOrder",
                principalTable: "Reading",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobOrders_Reading_JobOrder",
                table: "jobOrders");

            migrationBuilder.RenameColumn(
                name: "JobOrder",
                table: "jobOrders",
                newName: "MachineId");

            migrationBuilder.RenameIndex(
                name: "IX_jobOrders_JobOrder",
                table: "jobOrders",
                newName: "IX_jobOrders_MachineId");

            migrationBuilder.AddColumn<int>(
                name: "JobOrderId",
                table: "Reading",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "JobOrderId",
                table: "jobOrders",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Consumed",
                table: "jobOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "machines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MachineCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_machines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reading_JobOrderId",
                table: "Reading",
                column: "JobOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_jobOrders_machines_MachineId",
                table: "jobOrders",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reading_jobOrders_JobOrderId",
                table: "Reading",
                column: "JobOrderId",
                principalTable: "jobOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
