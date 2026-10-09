using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicWebsiteManagedStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicWebsiteManagedLocations",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FolderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteManagedLocations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicWebsiteManagedLocations_ParentCompanyId_RootKey_FolderName",
                schema: "GeneralSettings",
                table: "PublicWebsiteManagedLocations",
                columns: new[] { "ParentCompanyId", "RootKey", "FolderName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicWebsiteManagedLocations",
                schema: "GeneralSettings");
        }
    }
}
