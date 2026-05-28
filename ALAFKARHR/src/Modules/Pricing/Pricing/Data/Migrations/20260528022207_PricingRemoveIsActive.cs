using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pricing.Data.Migrations
{
    /// <inheritdoc />
    public partial class PricingRemoveIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceListItems_PriceListId_ProductSkuId_UnitId_MinQuantity_EffectiveFrom",
                schema: "Pricing",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                schema: "Pricing",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                schema: "Pricing",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Pricing",
                table: "PriceListItems");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_PriceListId_ProductSkuId_UnitId_MinQuantity",
                schema: "Pricing",
                table: "PriceListItems",
                columns: new[] { "PriceListId", "ProductSkuId", "UnitId", "MinQuantity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceListItems_PriceListId_ProductSkuId_UnitId_MinQuantity",
                schema: "Pricing",
                table: "PriceListItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                schema: "Pricing",
                table: "PriceListItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                schema: "Pricing",
                table: "PriceListItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Pricing",
                table: "PriceListItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_PriceListId_ProductSkuId_UnitId_MinQuantity_EffectiveFrom",
                schema: "Pricing",
                table: "PriceListItems",
                columns: new[] { "PriceListId", "ProductSkuId", "UnitId", "MinQuantity", "EffectiveFrom" });
        }
    }
}
