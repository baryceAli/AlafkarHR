using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Organization",
                table: "LicenseCategories",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Organization",
                table: "LicenseCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseCategories_CurrencyId",
                schema: "Organization",
                table: "LicenseCategories",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LicenseCategories_CurrencyId",
                schema: "Organization",
                table: "LicenseCategories");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Organization",
                table: "LicenseCategories");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Organization",
                table: "LicenseCategories",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
