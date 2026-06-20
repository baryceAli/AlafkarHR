using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationLicenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LicenseCategories",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    MaxChildCompanies = table.Column<int>(type: "int", nullable: false),
                    MaxBranches = table.Column<int>(type: "int", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLicenses_LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses",
                column: "LicenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseCategories_IsActive",
                schema: "Organization",
                table: "LicenseCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseCategories_Key",
                schema: "Organization",
                table: "LicenseCategories",
                column: "Key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyLicenses_LicenseCategories_LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses",
                column: "LicenseCategoryId",
                principalSchema: "Organization",
                principalTable: "LicenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyLicenses_LicenseCategories_LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses");

            migrationBuilder.DropTable(
                name: "LicenseCategories",
                schema: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_CompanyLicenses_LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses");

            migrationBuilder.DropColumn(
                name: "LicenseCategoryId",
                schema: "Organization",
                table: "CompanyLicenses");
        }
    }
}
