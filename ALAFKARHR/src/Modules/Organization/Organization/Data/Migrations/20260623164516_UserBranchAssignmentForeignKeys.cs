using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserBranchAssignmentForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAssignments_Branches_BranchId",
                schema: "Organization",
                table: "UserBranchAssignments",
                column: "BranchId",
                principalSchema: "Organization",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAssignments_Companies_CompanyId",
                schema: "Organization",
                table: "UserBranchAssignments",
                column: "CompanyId",
                principalSchema: "Organization",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAssignments_Branches_BranchId",
                schema: "Organization",
                table: "UserBranchAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAssignments_Companies_CompanyId",
                schema: "Organization",
                table: "UserBranchAssignments");
        }
    }
}
