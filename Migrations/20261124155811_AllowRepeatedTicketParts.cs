using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class AllowRepeatedTicketParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts");

            migrationBuilder.CreateIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts",
                columns: new[] { "ServiceTicketId", "PartId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts");

            migrationBuilder.CreateIndex(
                name: "IX_TicketParts_ServiceTicketId_PartId",
                table: "TicketParts",
                columns: new[] { "ServiceTicketId", "PartId" },
                unique: true);
        }
    }
}
