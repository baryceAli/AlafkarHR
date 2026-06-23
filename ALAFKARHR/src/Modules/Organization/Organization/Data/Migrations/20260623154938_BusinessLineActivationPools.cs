using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class BusinessLineActivationPools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivationLimit",
                schema: "Organization",
                table: "LicenseCategoryBusinessLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActivationLimit",
                schema: "Organization",
                table: "CompanyLicenseBusinessLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActivationPolicy",
                schema: "Organization",
                table: "BusinessLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusinessLineActivations",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLineActivations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLineActivations_BusinessLines_BusinessLineId",
                        column: x => x.BusinessLineId,
                        principalSchema: "Organization",
                        principalTable: "BusinessLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLineActivations_BusinessLineId",
                schema: "Organization",
                table: "BusinessLineActivations",
                column: "BusinessLineId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLineActivations_CompanyId_BusinessLineId_IsActive",
                schema: "Organization",
                table: "BusinessLineActivations",
                columns: new[] { "CompanyId", "BusinessLineId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLineActivations_ParentCompanyId_BusinessLineId_IsActive",
                schema: "Organization",
                table: "BusinessLineActivations",
                columns: new[] { "ParentCompanyId", "BusinessLineId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessLineActivations",
                schema: "Organization");

            migrationBuilder.DropColumn(
                name: "ActivationLimit",
                schema: "Organization",
                table: "LicenseCategoryBusinessLines");

            migrationBuilder.DropColumn(
                name: "ActivationLimit",
                schema: "Organization",
                table: "CompanyLicenseBusinessLines");

            migrationBuilder.DropColumn(
                name: "ActivationPolicy",
                schema: "Organization",
                table: "BusinessLines");
        }
    }
}
