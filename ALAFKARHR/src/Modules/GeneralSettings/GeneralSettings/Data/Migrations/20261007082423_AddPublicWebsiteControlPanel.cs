using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicWebsiteControlPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicWebsiteAudit",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    By = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteAudit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicWebsiteMedia",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    WasPublished = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteMedia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicWebsiteRevisions",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteRevisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicWebsiteSites",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublishedRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DraftJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteSites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicWebsiteAudit_CompanyId_At",
                schema: "GeneralSettings",
                table: "PublicWebsiteAudit",
                columns: new[] { "CompanyId", "At" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicWebsiteMedia_CompanyId",
                schema: "GeneralSettings",
                table: "PublicWebsiteMedia",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicWebsiteRevisions_CompanyId_PublishedAt",
                schema: "GeneralSettings",
                table: "PublicWebsiteRevisions",
                columns: new[] { "CompanyId", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicWebsiteSites_CompanyId",
                schema: "GeneralSettings",
                table: "PublicWebsiteSites",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicWebsiteAudit",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "PublicWebsiteMedia",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "PublicWebsiteRevisions",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "PublicWebsiteSites",
                schema: "GeneralSettings");
        }
    }
}
