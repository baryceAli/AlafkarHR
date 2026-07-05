using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventorySerialTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialNumbersCsv",
                schema: "Inventory",
                table: "TransferItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumbersCsv",
                schema: "Inventory",
                table: "CycleCountLines",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventorySerialNumberId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventorySerialNumbers",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastStockMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastMovementAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySerialNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockMovementSerials",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventorySerialNumberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StatusAfterMovement = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovementSerials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_InventorySerialNumberId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                column: "InventorySerialNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_SerialNumber",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySerialNumbers_CompanyId_ProductSkuId_SerialNumber",
                schema: "Inventory",
                table: "InventorySerialNumbers",
                columns: new[] { "CompanyId", "ProductSkuId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventorySerialNumbers_CompanyId_ProductSkuId_WarehouseId_WarehouseLocationId_BatchId_Status",
                schema: "Inventory",
                table: "InventorySerialNumbers",
                columns: new[] { "CompanyId", "ProductSkuId", "WarehouseId", "WarehouseLocationId", "BatchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InventorySerialNumbers_LastStockMovementId",
                schema: "Inventory",
                table: "InventorySerialNumbers",
                column: "LastStockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementSerials_InventorySerialNumberId",
                schema: "Inventory",
                table: "StockMovementSerials",
                column: "InventorySerialNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementSerials_StockMovementId",
                schema: "Inventory",
                table: "StockMovementSerials",
                column: "StockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementSerials_StockMovementId_InventorySerialNumberId",
                schema: "Inventory",
                table: "StockMovementSerials",
                columns: new[] { "StockMovementId", "InventorySerialNumberId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventorySerialNumbers",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "StockMovementSerials",
                schema: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeOperationLines_InventorySerialNumberId",
                schema: "Inventory",
                table: "BarcodeOperationLines");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeOperationLines_SerialNumber",
                schema: "Inventory",
                table: "BarcodeOperationLines");

            migrationBuilder.DropColumn(
                name: "SerialNumbersCsv",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "SerialNumbersCsv",
                schema: "Inventory",
                table: "CycleCountLines");

            migrationBuilder.DropColumn(
                name: "InventorySerialNumberId",
                schema: "Inventory",
                table: "BarcodeOperationLines");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                schema: "Inventory",
                table: "BarcodeOperationLines");
        }
    }
}
