using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompanyAwareRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "Auth",
                table: "AspNetRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateKey",
                schema: "Auth",
                table: "AspNetRoles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "Auth",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "TemplateKey",
                schema: "Auth",
                table: "AspNetRoles");
        }
    }
}
