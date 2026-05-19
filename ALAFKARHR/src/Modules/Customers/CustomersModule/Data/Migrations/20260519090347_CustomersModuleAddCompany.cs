using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomersModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomersModuleAddCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Customer",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Customer",
                table: "CustomerPricingProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Customer",
                table: "CustomerGroups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Customer",
                table: "CustomerPricingProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Customer",
                table: "CustomerGroups");
        }
    }
}
