using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class Fixedentityrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTickets_Equipments_EquipmentId",
                table: "ServiceTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTickets_Users_UserId",
                table: "ServiceTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketParts_Parts_PartId",
                table: "TicketParts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TicketPartId",
                table: "TicketParts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ServiceTicketId",
                table: "ServiceTickets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PartId",
                table: "Parts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FaultId",
                table: "Faults",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "Equipments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customers",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTickets_Equipments_EquipmentId",
                table: "ServiceTickets",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTickets_Users_UserId",
                table: "ServiceTickets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketParts_Parts_PartId",
                table: "TicketParts",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTickets_Equipments_EquipmentId",
                table: "ServiceTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTickets_Users_UserId",
                table: "ServiceTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketParts_Parts_PartId",
                table: "TicketParts");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TicketParts",
                newName: "TicketPartId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ServiceTickets",
                newName: "ServiceTicketId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Parts",
                newName: "PartId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Faults",
                newName: "FaultId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Equipments",
                newName: "EquipmentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTickets_Equipments_EquipmentId",
                table: "ServiceTickets",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTickets_Users_UserId",
                table: "ServiceTickets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketParts_Parts_PartId",
                table: "TicketParts",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "PartId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
