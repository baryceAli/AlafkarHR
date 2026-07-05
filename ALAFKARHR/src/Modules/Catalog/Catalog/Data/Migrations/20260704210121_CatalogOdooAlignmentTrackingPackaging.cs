using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogOdooAlignmentTrackingPackaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrackingMode",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "PurchaseEnabled",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<bool>(
                name: "SalesEnabled",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuPackages_Barcode",
                schema: "Catalog",
                table: "ProductSkuPackages",
                column: "Barcode",
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuPackages_UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSkuPackages_Units_UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages",
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
                name: "FK_ProductSkuPackages_Units_UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropIndex(
                name: "IX_ProductSkuPackages_Barcode",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropIndex(
                name: "IX_ProductSkuPackages_UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "TrackingMode",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "PurchaseEnabled",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "SalesEnabled",
                schema: "Catalog",
                table: "ProductSkuPackages");

            migrationBuilder.DropColumn(
                name: "UnitId",
                schema: "Catalog",
                table: "ProductSkuPackages");
        }
    }
}
