using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreFront.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoreFrontStoreManagerLogoAutomation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreManagerEmployeeId",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreManagerName",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreManagerNameEng",
                schema: "StoreFront",
                table: "StoreFronts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreFronts_StoreManagerEmployeeId",
                schema: "StoreFront",
                table: "StoreFronts",
                column: "StoreManagerEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreFronts_StoreManagerEmployeeId",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "StoreManagerEmployeeId",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "StoreManagerName",
                schema: "StoreFront",
                table: "StoreFronts");

            migrationBuilder.DropColumn(
                name: "StoreManagerNameEng",
                schema: "StoreFront",
                table: "StoreFronts");
        }
    }
}
