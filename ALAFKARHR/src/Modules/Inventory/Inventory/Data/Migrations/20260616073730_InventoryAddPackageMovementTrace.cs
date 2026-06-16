using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryAddPackageMovementTrace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EnteredQuantity",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NormalizedQuantity",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PackageMultiplier",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductPackageId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnteredQuantity",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "NormalizedQuantity",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "PackageMultiplier",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ProductPackageId",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
