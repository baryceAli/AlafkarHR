using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleet.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Fleet",
                table: "VehicleExpenses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleExpenses_CurrencyId",
                schema: "Fleet",
                table: "VehicleExpenses",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleExpenses_CurrencyId",
                schema: "Fleet",
                table: "VehicleExpenses");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Fleet",
                table: "VehicleExpenses");
        }
    }
}
