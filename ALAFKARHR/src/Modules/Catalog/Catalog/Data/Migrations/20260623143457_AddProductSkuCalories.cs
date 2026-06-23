using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSkuCalories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogNutritionSettings",
                schema: "Catalog");

            migrationBuilder.AddColumn<decimal>(
                name: "Calories",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calories",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.CreateTable(
                name: "CatalogNutritionSettings",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalorieVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogNutritionSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogNutritionSettings_CalorieVariantId",
                schema: "Catalog",
                table: "CatalogNutritionSettings",
                column: "CalorieVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogNutritionSettings_CompanyId",
                schema: "Catalog",
                table: "CatalogNutritionSettings",
                column: "CompanyId",
                unique: true);
        }
    }
}
