using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralSettings.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanySetupProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanySetupProfiles",
                schema: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaVersion = table.Column<int>(type: "int", nullable: false),
                    CurrentStepKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RemindersDismissed = table.Column<bool>(type: "bit", nullable: false),
                    SelectedTrackKeys = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SkippedStepKeys = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_CompanySetupProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySetupProfiles_CompanyId",
                schema: "GeneralSettings",
                table: "CompanySetupProfiles",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySetupProfiles",
                schema: "GeneralSettings");
        }
    }
}
