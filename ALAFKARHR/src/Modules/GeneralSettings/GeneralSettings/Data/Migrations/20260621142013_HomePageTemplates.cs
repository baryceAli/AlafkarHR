using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class HomePageTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignLandingHomePageContents",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltTextEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltTextAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CampaignLandingHomePageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorporateShowcaseHomePageContents",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltTextEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltTextAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CorporateShowcaseHomePageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentStorefrontHomePageContents",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltTextEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltTextAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CurrentStorefrontHomePageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomePageTemplateSelections",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveTemplateKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
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
                    table.PrimaryKey("PK_HomePageTemplateSelections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MinimalCatalogHomePageContents",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltTextEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltTextAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_MinimalCatalogHomePageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductHighlightHomePageContents",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltTextEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltTextAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ProductHighlightHomePageContents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignLandingHomePageContents_CompanyId",
                schema: "GeneralSettings",
                table: "CampaignLandingHomePageContents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignLandingHomePageContents_CompanyId_SectionKey_FieldKey",
                schema: "GeneralSettings",
                table: "CampaignLandingHomePageContents",
                columns: new[] { "CompanyId", "SectionKey", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateShowcaseHomePageContents_CompanyId",
                schema: "GeneralSettings",
                table: "CorporateShowcaseHomePageContents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateShowcaseHomePageContents_CompanyId_SectionKey_FieldKey",
                schema: "GeneralSettings",
                table: "CorporateShowcaseHomePageContents",
                columns: new[] { "CompanyId", "SectionKey", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrentStorefrontHomePageContents_CompanyId",
                schema: "GeneralSettings",
                table: "CurrentStorefrontHomePageContents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentStorefrontHomePageContents_CompanyId_SectionKey_FieldKey",
                schema: "GeneralSettings",
                table: "CurrentStorefrontHomePageContents",
                columns: new[] { "CompanyId", "SectionKey", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomePageTemplateSelections_CompanyId",
                schema: "GeneralSettings",
                table: "HomePageTemplateSelections",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinimalCatalogHomePageContents_CompanyId",
                schema: "GeneralSettings",
                table: "MinimalCatalogHomePageContents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MinimalCatalogHomePageContents_CompanyId_SectionKey_FieldKey",
                schema: "GeneralSettings",
                table: "MinimalCatalogHomePageContents",
                columns: new[] { "CompanyId", "SectionKey", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductHighlightHomePageContents_CompanyId",
                schema: "GeneralSettings",
                table: "ProductHighlightHomePageContents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHighlightHomePageContents_CompanyId_SectionKey_FieldKey",
                schema: "GeneralSettings",
                table: "ProductHighlightHomePageContents",
                columns: new[] { "CompanyId", "SectionKey", "FieldKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignLandingHomePageContents",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "CorporateShowcaseHomePageContents",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "CurrentStorefrontHomePageContents",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "HomePageTemplateSelections",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "MinimalCatalogHomePageContents",
                schema: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "ProductHighlightHomePageContents",
                schema: "GeneralSettings");
        }
    }
}
