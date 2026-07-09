using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase12InventoryScrapOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapOrders",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScrapLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SourceInventoryOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceInventoryOperationLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScrapOrderNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ReplenishQuantity = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrapOrderLines",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScrapOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScrapLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceInventoryOperationLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StockMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrapOrderLines_ScrapOrders_ScrapOrderId",
                        column: x => x.ScrapOrderId,
                        principalSchema: "Inventory",
                        principalTable: "ScrapOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrapOrderLineSerials",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScrapOrderLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventorySerialNumberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapOrderLineSerials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrapOrderLineSerials_ScrapOrderLines_ScrapOrderLineId",
                        column: x => x.ScrapOrderLineId,
                        principalSchema: "Inventory",
                        principalTable: "ScrapOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLines_ProductSkuId_BatchId",
                schema: "Inventory",
                table: "ScrapOrderLines",
                columns: new[] { "ProductSkuId", "BatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLines_ScrapOrderId_LineNumber",
                schema: "Inventory",
                table: "ScrapOrderLines",
                columns: new[] { "ScrapOrderId", "LineNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLines_SourceDocumentLineId",
                schema: "Inventory",
                table: "ScrapOrderLines",
                column: "SourceDocumentLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLines_SourceInventoryOperationLineId",
                schema: "Inventory",
                table: "ScrapOrderLines",
                column: "SourceInventoryOperationLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLines_StockMovementId",
                schema: "Inventory",
                table: "ScrapOrderLines",
                column: "StockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLineSerials_InventorySerialNumberId",
                schema: "Inventory",
                table: "ScrapOrderLineSerials",
                column: "InventorySerialNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrderLineSerials_ScrapOrderLineId_SerialNumber",
                schema: "Inventory",
                table: "ScrapOrderLineSerials",
                columns: new[] { "ScrapOrderLineId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrders_CompanyId_BranchId_Status",
                schema: "Inventory",
                table: "ScrapOrders",
                columns: new[] { "CompanyId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrders_CompanyId_ScrapOrderNumber",
                schema: "Inventory",
                table: "ScrapOrders",
                columns: new[] { "CompanyId", "ScrapOrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrders_CompanyId_WarehouseId_Status",
                schema: "Inventory",
                table: "ScrapOrders",
                columns: new[] { "CompanyId", "WarehouseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrders_SourceDocumentType_SourceDocumentId",
                schema: "Inventory",
                table: "ScrapOrders",
                columns: new[] { "SourceDocumentType", "SourceDocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapOrders_SourceInventoryOperationId",
                schema: "Inventory",
                table: "ScrapOrders",
                column: "SourceInventoryOperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapOrderLineSerials",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "ScrapOrderLines",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "ScrapOrders",
                schema: "Inventory");
        }
    }
}
