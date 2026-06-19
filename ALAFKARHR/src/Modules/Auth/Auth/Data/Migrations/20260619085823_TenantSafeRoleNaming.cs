using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class TenantSafeRoleNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CompanyId_DisplayName",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CompanyId_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                schema: "Auth",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                schema: "Auth",
                table: "AspNetRoles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CompanyId_DisplayName",
                schema: "Auth",
                table: "AspNetRoles",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true,
                filter: "[CompanyId] IS NOT NULL AND [DisplayName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CompanyId_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles",
                columns: new[] { "CompanyId", "TemplateKey" },
                unique: true,
                filter: "[CompanyId] IS NOT NULL AND [TemplateKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_Platform_DisplayName",
                schema: "Auth",
                table: "AspNetRoles",
                column: "DisplayName",
                unique: true,
                filter: "[CompanyId] IS NULL AND [DisplayName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_Platform_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles",
                column: "TemplateKey",
                unique: true,
                filter: "[CompanyId] IS NULL AND [TemplateKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CompanyId_DisplayName",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CompanyId_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_Platform_DisplayName",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_Platform_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                schema: "Auth",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                schema: "Auth",
                table: "AspNetRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CompanyId_DisplayName",
                schema: "Auth",
                table: "AspNetRoles",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true,
                filter: "[DisplayName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CompanyId_TemplateKey",
                schema: "Auth",
                table: "AspNetRoles",
                columns: new[] { "CompanyId", "TemplateKey" },
                unique: true,
                filter: "[TemplateKey] IS NOT NULL");
        }
    }
}
