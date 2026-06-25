using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreFront.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreFrontOrganizationPlacement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdministrationId",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_AdministrationId",
                schema: "StoreFront",
                table: "StoreFronts",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_DepartmentId",
                schema: "StoreFront",
                table: "StoreFronts",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreFronts_AdministrationId",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropIndex(
                name: "IX_StoreFronts_DepartmentId",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "StoreFront",
                table: "StoreFronts");
        }
    }
}
