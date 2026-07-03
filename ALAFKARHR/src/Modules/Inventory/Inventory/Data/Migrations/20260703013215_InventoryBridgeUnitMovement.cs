using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryBridgeUnitMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitMultiplier",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_UnitId",
                schema: "Inventory",
                table: "StockMovements",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockMovements_UnitId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UnitId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UnitMultiplier",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
