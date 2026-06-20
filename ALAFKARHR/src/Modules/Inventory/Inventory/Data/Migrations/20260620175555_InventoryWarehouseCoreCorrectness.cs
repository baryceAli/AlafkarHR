using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryWarehouseCoreCorrectness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BatchStocks_InventoryAggregateId",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.AlterColumn<string>(
                name: "TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Inventory",
                table: "TransferItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                schema: "Inventory",
                table: "TransferItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "SourceDocumentType",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Inventory",
                table: "Inventories",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Inventory",
                table: "BatchStocks",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "UX_WarehouseTransfer_Company_TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                columns: new[] { "CompanyId", "TransferNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceDocumentType_ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements",
                columns: new[] { "SourceDocumentType", "ReferenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId_ProductSkuId_BatchId",
                schema: "Inventory",
                table: "StockMovements",
                columns: new[] { "WarehouseId", "ProductSkuId", "BatchId" });

            migrationBuilder.CreateIndex(
                name: "UX_InventoryAggregate_Company_Warehouse_Sku",
                schema: "Inventory",
                table: "Inventories",
                columns: new[] { "CompanyId", "WarehouseId", "ProductSkuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_BatchStock_Inventory_Batch_Warehouse",
                schema: "Inventory",
                table: "BatchStocks",
                columns: new[] { "InventoryAggregateId", "BatchId", "WarehouseId" },
                unique: true,
                filter: "[InventoryAggregateId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_WarehouseTransfer_Company_TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_SourceDocumentType_ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_WarehouseId_ProductSkuId_BatchId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "UX_InventoryAggregate_Company_Warehouse_Sku",
                schema: "Inventory",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "UX_BatchStock_Inventory_Batch_Warehouse",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Inventory",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Inventory",
                table: "BatchStocks");

            migrationBuilder.AlterColumn<string>(
                name: "TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceDocumentType",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_BatchStocks_InventoryAggregateId",
                schema: "Inventory",
                table: "BatchStocks",
                column: "InventoryAggregateId");
        }
    }
}
