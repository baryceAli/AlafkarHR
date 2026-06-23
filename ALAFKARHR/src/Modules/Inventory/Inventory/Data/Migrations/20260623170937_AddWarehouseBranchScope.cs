using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseBranchScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CompanyId_BranchId",
                schema: "Inventory",
                table: "Warehouses",
                columns: new[] { "CompanyId", "BranchId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CompanyId_BranchId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Inventory",
                table: "Warehouses");
        }
    }
}
