using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pricing.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Pricing",
                table: "PriceLists",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Pricing",
                table: "PriceLists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_CurrencyId",
                schema: "Pricing",
                table: "PriceLists",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceLists_CurrencyId",
                schema: "Pricing",
                table: "PriceLists");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Pricing",
                table: "PriceLists");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Pricing",
                table: "PriceLists",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
