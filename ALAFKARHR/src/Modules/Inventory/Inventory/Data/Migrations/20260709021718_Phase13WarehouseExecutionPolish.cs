using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase13WarehouseExecutionPolish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryOperationId",
                schema: "Inventory",
                table: "BarcodeOperationSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryOperationLineId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PickingGroups",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    GroupNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsibleUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DockLocation = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_PickingGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickingGroupLines",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PickingGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDocumentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    OperationKind = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OperationStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DoneQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_PickingGroupLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingGroupLines_PickingGroups_PickingGroupId",
                        column: x => x.PickingGroupId,
                        principalSchema: "Inventory",
                        principalTable: "PickingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationSessions_InventoryOperationId",
                schema: "Inventory",
                table: "BarcodeOperationSessions",
                column: "InventoryOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeOperationLines_InventoryOperationLineId",
                schema: "Inventory",
                table: "BarcodeOperationLines",
                column: "InventoryOperationLineId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroupLines_InventoryOperationId",
                schema: "Inventory",
                table: "PickingGroupLines",
                column: "InventoryOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroupLines_PickingGroupId_InventoryOperationId",
                schema: "Inventory",
                table: "PickingGroupLines",
                columns: new[] { "PickingGroupId", "InventoryOperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroupLines_SourceDocumentType_SourceDocumentId",
                schema: "Inventory",
                table: "PickingGroupLines",
                columns: new[] { "SourceDocumentType", "SourceDocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroups_CompanyId_BranchId_Status",
                schema: "Inventory",
                table: "PickingGroups",
                columns: new[] { "CompanyId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroups_CompanyId_GroupNumber",
                schema: "Inventory",
                table: "PickingGroups",
                columns: new[] { "CompanyId", "GroupNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingGroups_CompanyId_WarehouseId_GroupType_Status",
                schema: "Inventory",
                table: "PickingGroups",
                columns: new[] { "CompanyId", "WarehouseId", "GroupType", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickingGroupLines",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "PickingGroups",
                schema: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeOperationSessions_InventoryOperationId",
                schema: "Inventory",
                table: "BarcodeOperationSessions");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeOperationLines_InventoryOperationLineId",
                schema: "Inventory",
                table: "BarcodeOperationLines");

            migrationBuilder.DropColumn(
                name: "InventoryOperationId",
                schema: "Inventory",
                table: "BarcodeOperationSessions");

            migrationBuilder.DropColumn(
                name: "InventoryOperationLineId",
                schema: "Inventory",
                table: "BarcodeOperationLines");
        }
    }
}
