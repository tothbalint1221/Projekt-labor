using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class faultConfiged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faults_Equipments_EquipmentId",
                table: "Faults");

            migrationBuilder.DropIndex(
                name: "IX_TicketParts_ServiceTicketId",
                table: "TicketParts");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "Faults",
                newName: "ServiceTicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Faults_EquipmentId",
                table: "Faults",
                newName: "IX_Faults_ServiceTicketId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "TicketParts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts",
                columns: new[] { "ServiceTicketId", "PartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Faults_ServiceTickets_ServiceTicketId",
                table: "Faults",
                column: "ServiceTicketId",
                principalTable: "ServiceTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faults_ServiceTickets_ServiceTicketId",
                table: "Faults");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "TicketParts");

            migrationBuilder.RenameColumn(
                name: "ServiceTicketId",
                table: "Faults",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Faults_ServiceTicketId",
                table: "Faults",
                newName: "IX_Faults_EquipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_TicketParts_ServiceTicketId",
                table: "TicketParts",
                column: "ServiceTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Faults_Equipments_EquipmentId",
                table: "Faults",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
