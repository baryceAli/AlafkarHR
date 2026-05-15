using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class SettingsAddIsDefaultAndCompanyToCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "GeneralSettings",
                table: "Currencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "GeneralSettings",
                table: "Currencies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "GeneralSettings",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "GeneralSettings",
                table: "Currencies");
        }
    }
}
