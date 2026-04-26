using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "Variants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "Units",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "ProductPackages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Catalog",
                table: "Brands",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Catalog",
                table: "Brands");
        }
    }
}
