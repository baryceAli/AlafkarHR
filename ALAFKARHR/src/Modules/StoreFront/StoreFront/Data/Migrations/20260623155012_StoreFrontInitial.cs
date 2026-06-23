using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreFront.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoreFrontInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "StoreFront");

            migrationBuilder.CreateTable(
                name: "StoreFrontTypes",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_StoreFrontTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreFronts",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreFrontTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriceListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceiptHeader = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReceiptFooter = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_StoreFronts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreFronts_StoreFrontTypes_StoreFrontTypeId",
                        column: x => x.StoreFrontTypeId,
                        principalSchema: "StoreFront",
                        principalTable: "StoreFrontTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreFrontSellableItems",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreFrontId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProductNameEng = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SkuCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    AllowManualPrice = table.Column<bool>(type: "bit", nullable: false),
                    RequireManualPriceNote = table.Column<bool>(type: "bit", nullable: false),
                    MinimumManualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumManualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                    table.PrimaryKey("PK_StoreFrontSellableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreFrontSellableItems_StoreFronts_StoreFrontId",
                        column: x => x.StoreFrontId,
                        principalSchema: "StoreFront",
                        principalTable: "StoreFronts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_CompanyId_Code",
                schema: "StoreFront",
                table: "StoreFronts",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_CompanyId_IsActive",
                schema: "StoreFront",
                table: "StoreFronts",
                columns: new[] { "CompanyId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_StoreFrontTypeId",
                schema: "StoreFront",
                table: "StoreFronts",
                column: "StoreFrontTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreFrontSellableItems_ProductSkuId",
                schema: "StoreFront",
                table: "StoreFrontSellableItems",
                column: "ProductSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreFrontSellableItems_StoreFrontId_ProductSkuId",
                schema: "StoreFront",
                table: "StoreFrontSellableItems",
                columns: new[] { "StoreFrontId", "ProductSkuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreFrontTypes_CompanyId_Code",
                schema: "StoreFront",
                table: "StoreFrontTypes",
                columns: new[] { "CompanyId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreFrontSellableItems",
                schema: "StoreFront");

            migrationBuilder.DropTable(
                name: "StoreFronts",
                schema: "StoreFront");

            migrationBuilder.DropTable(
                name: "StoreFrontTypes",
                schema: "StoreFront");
        }
    }
}
