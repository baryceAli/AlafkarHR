using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase11ReplenishmentCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HorizonDays",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastGeneratedAt",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastGeneratedDocumentId",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastGeneratedDocumentNumber",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRunAt",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MultipleQuantity",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TriggerMode",
                schema: "Procurement",
                table: "ReorderingRules",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Manual");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorizonDays",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "LastGeneratedAt",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "LastGeneratedDocumentId",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "LastGeneratedDocumentNumber",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "LastRunAt",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "MultipleQuantity",
                schema: "Procurement",
                table: "ReorderingRules");

            migrationBuilder.DropColumn(
                name: "TriggerMode",
                schema: "Procurement",
                table: "ReorderingRules");
        }
    }
}
