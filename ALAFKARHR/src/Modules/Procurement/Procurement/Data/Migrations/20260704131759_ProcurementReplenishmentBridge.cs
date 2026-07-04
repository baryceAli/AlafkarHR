using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProcurementReplenishmentBridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReorderingRuleId",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementDocumentLines_ReorderingRuleId",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                column: "ReorderingRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcurementDocumentLines_ReorderingRuleId",
                schema: "Procurement",
                table: "ProcurementDocumentLines");

            migrationBuilder.DropColumn(
                name: "ReorderingRuleId",
                schema: "Procurement",
                table: "ProcurementDocumentLines");
        }
    }
}
