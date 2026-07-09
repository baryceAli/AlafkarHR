using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase8WarehouseConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultDestinationLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultOutputLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPackingLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultQualityLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultSourceLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultTransitLocationId",
                schema: "Inventory",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InboundFlow",
                schema: "Inventory",
                table: "Warehouses",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "OneStep");

            migrationBuilder.AddColumn<string>(
                name: "OutboundFlow",
                schema: "Inventory",
                table: "Warehouses",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "OneStep");

            migrationBuilder.AddColumn<string>(
                name: "ShortCode",
                schema: "Inventory",
                table: "Warehouses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WarehouseResupplyLinks",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_WarehouseResupplyLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseResupplyLinks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "Inventory",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CompanyId_ShortCode",
                schema: "Inventory",
                table: "Warehouses",
                columns: new[] { "CompanyId", "ShortCode" },
                filter: "[ShortCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseResupplyLinks_CompanyId_SourceWarehouseId",
                schema: "Inventory",
                table: "WarehouseResupplyLinks",
                columns: new[] { "CompanyId", "SourceWarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseResupplyLinks_CompanyId_WarehouseId_SourceWarehouseId",
                schema: "Inventory",
                table: "WarehouseResupplyLinks",
                columns: new[] { "CompanyId", "WarehouseId", "SourceWarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseResupplyLinks_WarehouseId",
                schema: "Inventory",
                table: "WarehouseResupplyLinks",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseResupplyLinks",
                schema: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CompanyId_ShortCode",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultDestinationLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultOutputLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultPackingLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultQualityLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultSourceLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "DefaultTransitLocationId",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "InboundFlow",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "OutboundFlow",
                schema: "Inventory",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ShortCode",
                schema: "Inventory",
                table: "Warehouses");
        }
    }
}
