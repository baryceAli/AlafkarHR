using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Data.Migrations
{
    /// <inheritdoc />
    public partial class MaintenanceAssetSourceLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceEntityId",
                schema: "Maintenance",
                table: "MaintenanceAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceEntityName",
                schema: "Maintenance",
                table: "MaintenanceAssets",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceModule",
                schema: "Maintenance",
                table: "MaintenanceAssets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceAssets_SourceModule_SourceEntityName_SourceEntityId",
                schema: "Maintenance",
                table: "MaintenanceAssets",
                columns: new[] { "SourceModule", "SourceEntityName", "SourceEntityId" },
                unique: true,
                filter: "[SourceModule] IS NOT NULL AND [SourceEntityName] IS NOT NULL AND [SourceEntityId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaintenanceAssets_SourceModule_SourceEntityName_SourceEntityId",
                schema: "Maintenance",
                table: "MaintenanceAssets");

            migrationBuilder.DropColumn(
                name: "SourceEntityId",
                schema: "Maintenance",
                table: "MaintenanceAssets");

            migrationBuilder.DropColumn(
                name: "SourceEntityName",
                schema: "Maintenance",
                table: "MaintenanceAssets");

            migrationBuilder.DropColumn(
                name: "SourceModule",
                schema: "Maintenance",
                table: "MaintenanceAssets");
        }
    }
}
