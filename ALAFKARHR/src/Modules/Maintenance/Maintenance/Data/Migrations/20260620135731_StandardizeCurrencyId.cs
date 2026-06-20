using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Maintenance",
                table: "MaintenanceWorkOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceWorkOrders_CurrencyId",
                schema: "Maintenance",
                table: "MaintenanceWorkOrders",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaintenanceWorkOrders_CurrencyId",
                schema: "Maintenance",
                table: "MaintenanceWorkOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Maintenance",
                table: "MaintenanceWorkOrders");
        }
    }
}
