using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryAddCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchStocks_BatchStocks_BatchStockId",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.DropIndex(
                name: "IX_BatchStocks_BatchStockId",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.DropColumn(
                name: "MovementCategory",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "MovementDate",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "BatchStockId",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                schema: "Inventory",
                table: "StockMovements",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "Inventory",
                table: "StockMovements",
                newName: "QuantityBefore");

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityAfter",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ReservedAfter",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReservedBefore",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SourceDocumentType",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Inventory",
                table: "Inventories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityAfter",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReservedAfter",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReservedBefore",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceDocumentType",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Inventory",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "QuantityBefore",
                schema: "Inventory",
                table: "StockMovements",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Currency",
                schema: "Inventory",
                table: "StockMovements",
                newName: "ReferenceId");

            migrationBuilder.AddColumn<int>(
                name: "MovementCategory",
                schema: "Inventory",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovementDate",
                schema: "Inventory",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "BatchStockId",
                schema: "Inventory",
                table: "BatchStocks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchStocks_BatchStockId",
                schema: "Inventory",
                table: "BatchStocks",
                column: "BatchStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchStocks_BatchStocks_BatchStockId",
                schema: "Inventory",
                table: "BatchStocks",
                column: "BatchStockId",
                principalSchema: "Inventory",
                principalTable: "BatchStocks",
                principalColumn: "Id");
        }
    }
}
