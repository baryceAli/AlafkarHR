using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreFront.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoreFrontDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreFrontDepartments",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreFrontId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_StoreFrontDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreFrontDepartments_StoreFronts_StoreFrontId",
                        column: x => x.StoreFrontId,
                        principalSchema: "StoreFront",
                        principalTable: "StoreFronts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreFrontDepartments_CompanyId_StoreFrontId_IsActive",
                schema: "StoreFront",
                table: "StoreFrontDepartments",
                columns: new[] { "CompanyId", "StoreFrontId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreFrontDepartments_StoreFrontId_Code",
                schema: "StoreFront",
                table: "StoreFrontDepartments",
                columns: new[] { "StoreFrontId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreFrontDepartments",
                schema: "StoreFront");
        }
    }
}
