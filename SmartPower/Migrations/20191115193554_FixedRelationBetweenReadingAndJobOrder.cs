using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartPower.Migrations
{
    public partial class FixedRelationBetweenReadingAndJobOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobOrders_Reading_JobOrder",
                table: "jobOrders");

            migrationBuilder.RenameColumn(
                name: "JobOrder",
                table: "jobOrders",
                newName: "ReadingId");

            migrationBuilder.RenameIndex(
                name: "IX_jobOrders_JobOrder",
                table: "jobOrders",
                newName: "IX_jobOrders_ReadingId");

            migrationBuilder.AddForeignKey(
                name: "FK_jobOrders_Reading_ReadingId",
                table: "jobOrders",
                column: "ReadingId",
                principalTable: "Reading",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobOrders_Reading_ReadingId",
                table: "jobOrders");

            migrationBuilder.RenameColumn(
                name: "ReadingId",
                table: "jobOrders",
                newName: "JobOrder");

            migrationBuilder.RenameIndex(
                name: "IX_jobOrders_ReadingId",
                table: "jobOrders",
                newName: "IX_jobOrders_JobOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_jobOrders_Reading_JobOrder",
                table: "jobOrders",
                column: "JobOrder",
                principalTable: "Reading",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
