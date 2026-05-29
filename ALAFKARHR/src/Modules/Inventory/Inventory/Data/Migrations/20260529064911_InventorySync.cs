using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventorySync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferItems_WarehouseTransfers_WarehouseTransferId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_TransferItems_WarehouseId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReceivedQuantity",
                schema: "Inventory",
                table: "TransferItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfers_DestinationWarehouseId",
                schema: "Inventory",
                table: "WarehouseTransfers",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfers_SourceWarehouseId",
                schema: "Inventory",
                table: "WarehouseTransfers",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfers_Status",
                schema: "Inventory",
                table: "WarehouseTransfers",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferItems_WarehouseTransfers_WarehouseTransferId",
                schema: "Inventory",
                table: "TransferItems",
                column: "WarehouseTransferId",
                principalSchema: "Inventory",
                principalTable: "WarehouseTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferItems_WarehouseTransfers_WarehouseTransferId",
                schema: "Inventory",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransfers_DestinationWarehouseId",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransfers_SourceWarehouseId",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransfers_Status",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryDate",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.DropColumn(
                name: "TransferNumber",
                schema: "Inventory",
                table: "WarehouseTransfers");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "Inventory",
                table: "WarehouseTransfers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReceivedQuantity",
                schema: "Inventory",
                table: "TransferItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                schema: "Inventory",
                table: "TransferItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_WarehouseId",
                schema: "Inventory",
                table: "TransferItems",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferItems_WarehouseTransfers_WarehouseTransferId",
                schema: "Inventory",
                table: "TransferItems",
                column: "WarehouseTransferId",
                principalSchema: "Inventory",
                principalTable: "WarehouseTransfers",
                principalColumn: "Id");
        }
    }
}
