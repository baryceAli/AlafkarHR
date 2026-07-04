using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryBarcodeScanSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodeOperationSessions",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarningsJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_BarcodeOperationSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeOperationLines",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BarcodeOperationSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RawBarcode = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnteredQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PackageMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NormalizedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DisplayLabel = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DisplayLabelEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Warning = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_BarcodeOperationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarcodeOperationLines_BarcodeOperationSessions_BarcodeOperationSessionId",
                        column: x => x.BarcodeOperationSessionId,
                        principalSchema: "Inventory",
                        principalTable: "BarcodeOperationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_BarcodeOperationSessionId_EntityType",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                columns: new[] { "BarcodeOperationSessionId", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_BatchId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_ProductSkuId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                column: "ProductSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationSessions_CompanyId_OperationType_Status",
                schema: "Inventory",
                table: "BarcodeOperationSessions",
                columns: new[] { "CompanyId", "OperationType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationSessions_CompanyId_ReferenceNumber",
                schema: "Inventory",
                table: "BarcodeOperationSessions",
                columns: new[] { "CompanyId", "ReferenceNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeOperationLines",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "BarcodeOperationSessions",
                schema: "Inventory");
        }
    }
}
