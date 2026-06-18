using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryAssetInstancesAndWarehouseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseType",
                schema: "Inventory",
                table: "Warehouses",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Commercial");

            migrationBuilder.CreateTable(
                name: "AssetInstances",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_AssetInstances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_BranchId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_CompanyId_AssetTag",
                schema: "Inventory",
                table: "AssetInstances",
                columns: new[] { "CompanyId", "AssetTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_CompanyId_SerialNumber",
                schema: "Inventory",
                table: "AssetInstances",
                columns: new[] { "CompanyId", "SerialNumber" },
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_DepartmentId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_EmployeeId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_MaintenanceAssetId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "MaintenanceAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_ProductSkuId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "ProductSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInstances_WarehouseId",
                schema: "Inventory",
                table: "AssetInstances",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetInstances",
                schema: "Inventory");

            migrationBuilder.DropColumn(
                name: "WarehouseType",
                schema: "Inventory",
                table: "Warehouses");
        }
    }
}
