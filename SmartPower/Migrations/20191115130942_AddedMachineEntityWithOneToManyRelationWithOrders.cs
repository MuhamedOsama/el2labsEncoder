using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartPower.Migrations
{
    public partial class AddedMachineEntityWithOneToManyRelationWithOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobOrder_Reading_JobOrderId",
                table: "jobOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobOrder",
                table: "jobOrder");

            migrationBuilder.DropIndex(
                name: "IX_jobOrder_JobOrderId",
                table: "jobOrder");

            migrationBuilder.RenameTable(
                name: "jobOrder",
                newName: "jobOrders");

            migrationBuilder.AddColumn<bool>(
                name: "Consumed",
                table: "jobOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MachineId",
                table: "jobOrders",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobOrders",
                table: "jobOrders",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_jobOrders_MachineId",
                table: "jobOrders",
                column: "MachineId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobOrders",
                table: "jobOrders");

            migrationBuilder.DropIndex(
                name: "IX_jobOrders_MachineId",
                table: "jobOrders");

            migrationBuilder.DropColumn(
                name: "Consumed",
                table: "jobOrders");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "jobOrders");

            migrationBuilder.RenameTable(
                name: "jobOrders",
                newName: "jobOrder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobOrder",
                table: "jobOrder",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_jobOrder_JobOrderId",
                table: "jobOrder",
                column: "JobOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_jobOrder_Reading_JobOrderId",
                table: "jobOrder",
                column: "JobOrderId",
                principalTable: "Reading",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
