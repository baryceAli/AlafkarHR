using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase10InventoryOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryOperations",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowDirection = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OperationKind = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsStockPostingStep = table.Column<bool>(type: "bit", nullable: false),
                    StockPosted = table.Column<bool>(type: "bit", nullable: false),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDocumentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    BackorderOfOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_InventoryOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryOperationLines",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StockMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DoneQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConsumeReservedQuantity = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_InventoryOperationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryOperationLines_InventoryOperations_InventoryOperationId",
                        column: x => x.InventoryOperationId,
                        principalSchema: "Inventory",
                        principalTable: "InventoryOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperationLines_InventoryOperationId_ProductSkuId_BatchId",
                schema: "Inventory",
                table: "InventoryOperationLines",
                columns: new[] { "InventoryOperationId", "ProductSkuId", "BatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperationLines_SourceDocumentLineId",
                schema: "Inventory",
                table: "InventoryOperationLines",
                column: "SourceDocumentLineId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperations_CompanyId_BranchId_Status",
                schema: "Inventory",
                table: "InventoryOperations",
                columns: new[] { "CompanyId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperations_CompanyId_SourceDocumentType_SourceDocumentId_Sequence",
                schema: "Inventory",
                table: "InventoryOperations",
                columns: new[] { "CompanyId", "SourceDocumentType", "SourceDocumentId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperations_CompanyId_WarehouseId_Status_OperationKind",
                schema: "Inventory",
                table: "InventoryOperations",
                columns: new[] { "CompanyId", "WarehouseId", "Status", "OperationKind" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryOperationLines",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "InventoryOperations",
                schema: "Inventory");
        }
    }
}
