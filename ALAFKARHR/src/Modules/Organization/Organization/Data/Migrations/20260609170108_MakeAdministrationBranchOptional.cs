using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeAdministrationBranchOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrations_Branches_BranchId",
                schema: "Organization",
                table: "Administrations");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                schema: "Organization",
                table: "Administrations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrations_Branches_BranchId",
                schema: "Organization",
                table: "Administrations",
                column: "BranchId",
                principalSchema: "Organization",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrations_Branches_BranchId",
                schema: "Organization",
                table: "Administrations");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                schema: "Organization",
                table: "Administrations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Administrations_Branches_BranchId",
                schema: "Organization",
                table: "Administrations",
                column: "BranchId",
                principalSchema: "Organization",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
