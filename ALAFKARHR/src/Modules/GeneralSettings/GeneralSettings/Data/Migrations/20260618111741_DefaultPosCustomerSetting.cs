using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultPosCustomerSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPosCustomerId",
                schema: "GeneralSettings",
                table: "CompanySettings",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPosCustomerId",
                schema: "GeneralSettings",
                table: "CompanySettings");
        }
    }
}
