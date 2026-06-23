using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessLines",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_BusinessLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLicenseBusinessLines",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyLicenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLicenseBusinessLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLicenseBusinessLines_BusinessLines_BusinessLineId",
                        column: x => x.BusinessLineId,
                        principalSchema: "Organization",
                        principalTable: "BusinessLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyLicenseBusinessLines_CompanyLicenses_CompanyLicenseId",
                        column: x => x.CompanyLicenseId,
                        principalSchema: "Organization",
                        principalTable: "CompanyLicenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseCategoryBusinessLines",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseCategoryBusinessLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseCategoryBusinessLines_BusinessLines_BusinessLineId",
                        column: x => x.BusinessLineId,
                        principalSchema: "Organization",
                        principalTable: "BusinessLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseCategoryBusinessLines_LicenseCategories_LicenseCategoryId",
                        column: x => x.LicenseCategoryId,
                        principalSchema: "Organization",
                        principalTable: "LicenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLines_IsActive_DisplayOrder",
                schema: "Organization",
                table: "BusinessLines",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLines_Key",
                schema: "Organization",
                table: "BusinessLines",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLicenseBusinessLines_BusinessLineId",
                schema: "Organization",
                table: "CompanyLicenseBusinessLines",
                column: "BusinessLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLicenseBusinessLines_CompanyLicenseId_BusinessLineId",
                schema: "Organization",
                table: "CompanyLicenseBusinessLines",
                columns: new[] { "CompanyLicenseId", "BusinessLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseCategoryBusinessLines_BusinessLineId",
                schema: "Organization",
                table: "LicenseCategoryBusinessLines",
                column: "BusinessLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseCategoryBusinessLines_LicenseCategoryId_BusinessLineId",
                schema: "Organization",
                table: "LicenseCategoryBusinessLines",
                columns: new[] { "LicenseCategoryId", "BusinessLineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyLicenseBusinessLines",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "LicenseCategoryBusinessLines",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "BusinessLines",
                schema: "Organization");
        }
    }
}
