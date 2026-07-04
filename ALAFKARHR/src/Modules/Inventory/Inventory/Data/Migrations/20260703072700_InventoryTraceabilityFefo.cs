using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryTraceabilityFefo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentProductSkuId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentSalesOrderLineId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceDocumentId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceDocumentLineId",
                schema: "Inventory",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_BatchId",
                schema: "Inventory",
                table: "StockMovements",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ParentProductSkuId",
                schema: "Inventory",
                table: "StockMovements",
                column: "ParentProductSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ParentSalesOrderLineId",
                schema: "Inventory",
                table: "StockMovements",
                column: "ParentSalesOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceDocumentType_SourceDocumentId",
                schema: "Inventory",
                table: "StockMovements",
                columns: new[] { "SourceDocumentType", "SourceDocumentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Batches_BatchId",
                schema: "Inventory",
                table: "StockMovements",
                column: "BatchId",
                principalSchema: "Inventory",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Batches_BatchId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_BatchId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_ParentProductSkuId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_ParentSalesOrderLineId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_SourceDocumentType_SourceDocumentId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ParentProductSkuId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ParentSalesOrderLineId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceDocumentId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SourceDocumentLineId",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
