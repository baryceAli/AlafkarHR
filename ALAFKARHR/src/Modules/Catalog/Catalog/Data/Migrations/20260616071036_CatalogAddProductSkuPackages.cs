using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogAddProductSkuPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSkuPackages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ProductSkuPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSkuPackages_ProductPackages_ProductPackageId",
                        column: x => x.ProductPackageId,
                        principalSchema: "Catalog",
                        principalTable: "ProductPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSkuPackages_ProductSKUs_ProductSkuId",
                        column: x => x.ProductSkuId,
                        principalSchema: "Catalog",
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuPackages_ProductPackageId",
                schema: "Catalog",
                table: "ProductSkuPackages",
                column: "ProductPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSkuPackages_ProductSkuId_ProductPackageId",
                schema: "Catalog",
                table: "ProductSkuPackages",
                columns: new[] { "ProductSkuId", "ProductPackageId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.Sql("""
                INSERT INTO [Catalog].[ProductSkuPackages]
                    ([Id], [ProductSkuId], [ProductPackageId], [CreatedAt], [CreatedBy], [IsDeleted])
                SELECT
                    NEWID(),
                    [Id],
                    [PackageId],
                    COALESCE([CreatedAt], SYSUTCDATETIME()),
                    [CreatedBy],
                    CAST(0 AS bit)
                FROM [Catalog].[ProductSKUs]
                WHERE [PackageId] IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM [Catalog].[ProductSkuPackages] psp
                      WHERE psp.[ProductSkuId] = [Catalog].[ProductSKUs].[Id]
                        AND psp.[ProductPackageId] = [Catalog].[ProductSKUs].[PackageId]
                        AND psp.[IsDeleted] = 0
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSkuPackages",
                schema: "Catalog");
        }
    }
}
