using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentUploadPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentUploadPolicies",
                schema: "DocumentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxFileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    AllowedExtensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowedContentTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_DocumentUploadPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploadPolicies_CompanyId",
                schema: "DocumentManagement",
                table: "DocumentUploadPolicies",
                column: "CompanyId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentUploadPolicies",
                schema: "DocumentManagement");
        }
    }
}
