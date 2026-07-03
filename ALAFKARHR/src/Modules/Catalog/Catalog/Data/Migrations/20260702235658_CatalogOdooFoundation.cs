using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogOdooFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreationMode",
                schema: "Catalog",
                table: "Variants",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "DisplayType",
                schema: "Catalog",
                table: "Variants",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor",
                schema: "Catalog",
                table: "Units",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<bool>(
                name: "IsReferenceUnit",
                schema: "Catalog",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UnitCategory",
                schema: "Catalog",
                table: "Units",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                schema: "Catalog",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "Catalog",
                table: "ProductPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                schema: "Catalog",
                table: "ProductPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_CompanyId_UnitCategory_IsReferenceUnit",
                schema: "Catalog",
                table: "Units",
                columns: new[] { "CompanyId", "UnitCategory", "IsReferenceUnit" },
                unique: true,
                filter: "[IsReferenceUnit] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSKUs_CompanyId_ProductId_BrandId_PackageId_SkuCode",
                schema: "Catalog",
                table: "ProductSKUs",
                columns: new[] { "CompanyId", "ProductId", "BrandId", "PackageId", "SkuCode" },
                unique: true,
                filter: "[PackageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPackages_CompanyId_Barcode",
                schema: "Catalog",
                table: "ProductPackages",
                columns: new[] { "CompanyId", "Barcode" },
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPackages_UnitId",
                schema: "Catalog",
                table: "ProductPackages",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPackages_Units_UnitId",
                schema: "Catalog",
                table: "ProductPackages",
                column: "UnitId",
                principalSchema: "Catalog",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPackages_Units_UnitId",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropIndex(
                name: "IX_Units_CompanyId_UnitCategory_IsReferenceUnit",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_ProductSKUs_CompanyId_ProductId_BrandId_PackageId_SkuCode",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropIndex(
                name: "IX_ProductPackages_CompanyId_Barcode",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropIndex(
                name: "IX_ProductPackages_UnitId",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "CreationMode",
                schema: "Catalog",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "DisplayType",
                schema: "Catalog",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "ConversionFactor",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IsReferenceUnit",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "UnitCategory",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ProductType",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "UnitId",
                schema: "Catalog",
                table: "ProductPackages");
        }
    }
}
