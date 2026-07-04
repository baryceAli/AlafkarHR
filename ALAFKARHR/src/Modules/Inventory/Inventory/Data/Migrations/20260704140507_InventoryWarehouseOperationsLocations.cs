using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryWarehouseOperationsLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DestinationLocationId",
                schema: "Inventory",
                table: "TransferItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceLocationId",
                schema: "Inventory",
                table: "TransferItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationLocationId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceLocationId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_DestinationLocationId",
                schema: "Inventory",
                table: "TransferItems",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_SourceLocationId",
                schema: "Inventory",
                table: "TransferItems",
                column: "SourceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DestinationLocationId",
                schema: "Inventory",
                table: "StockMovements",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceLocationId",
                schema: "Inventory",
                table: "StockMovements",
                column: "SourceLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransferItems_DestinationLocationId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_TransferItems_SourceLocationId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_DestinationLocationId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_SourceLocationId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DestinationLocationId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "SourceLocationId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "DestinationLocationId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceLocationId",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
