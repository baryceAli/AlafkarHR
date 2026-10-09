using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicWebsiteConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicWebsiteConfiguration",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OwnerCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdministratorParentCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageLocationKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublicOrigin = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ImageLimitMiB = table.Column<int>(type: "int", nullable: false),
                    PdfLimitMiB = table.Column<int>(type: "int", nullable: false),
                    MediaLimitMiB = table.Column<int>(type: "int", nullable: false),
                    Activated = table.Column<bool>(type: "bit", nullable: false),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicWebsiteConfiguration", x => x.Id);
                    table.CheckConstraint("CK_PublicWebsiteConfiguration_Singleton", "[Id] = 1");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicWebsiteConfiguration",
                schema: "GeneralSettings");
        }
    }
}
