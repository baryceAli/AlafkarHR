using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogAddProductSkuComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSkuComponents",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
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
                    table.PrimaryKey("PK_ProductSkuComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSkuComponents_ProductSKUs_ComponentProductSkuId",
                        column: x => x.ComponentProductSkuId,
                        principalSchema: "Catalog",
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSkuComponents_ProductSKUs_ParentProductSkuId",
                        column: x => x.ParentProductSkuId,
                        principalSchema: "Catalog",
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuComponents_ComponentProductSkuId",
                schema: "Catalog",
                table: "ProductSkuComponents",
                column: "ComponentProductSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuComponents_ParentProductSkuId_ComponentProductSkuId",
                schema: "Catalog",
                table: "ProductSkuComponents",
                columns: new[] { "ParentProductSkuId", "ComponentProductSkuId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSkuComponents",
                schema: "Catalog");
        }
    }
}
