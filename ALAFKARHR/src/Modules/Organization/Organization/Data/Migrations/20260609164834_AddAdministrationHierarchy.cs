using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrationHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentAdministrationId",
                schema: "Organization",
                table: "Administrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrations_ParentAdministrationId",
                schema: "Organization",
                table: "Administrations",
                column: "ParentAdministrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrations_Administrations_ParentAdministrationId",
                schema: "Organization",
                table: "Administrations",
                column: "ParentAdministrationId",
                principalSchema: "Organization",
                principalTable: "Administrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrations_Administrations_ParentAdministrationId",
                schema: "Organization",
                table: "Administrations");

            migrationBuilder.DropIndex(
                name: "IX_Administrations_ParentAdministrationId",
                schema: "Organization",
                table: "Administrations");

            migrationBuilder.DropColumn(
                name: "ParentAdministrationId",
                schema: "Organization",
                table: "Administrations");
        }
    }
}
