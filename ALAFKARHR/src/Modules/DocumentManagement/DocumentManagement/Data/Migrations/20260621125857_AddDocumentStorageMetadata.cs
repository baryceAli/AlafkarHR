using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentStorageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                schema: "DocumentManagement",
                table: "DocumentVersions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageProvider",
                schema: "DocumentManagement",
                table: "DocumentVersions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "LocalFileSystem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageKey",
                schema: "DocumentManagement",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "StorageProvider",
                schema: "DocumentManagement",
                table: "DocumentVersions");
        }
    }
}
