using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectManagementSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskLinks_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectTaskLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaterialRequirements_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectMaterialRequirements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHandoffs_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectHandoffs",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHandoffs_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectHandoffs",
                column: "ProjectId",
                principalSchema: "ProjectManagement",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaterialRequirements_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectMaterialRequirements",
                column: "ProjectId",
                principalSchema: "ProjectManagement",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskLinks_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectTaskLinks",
                column: "ProjectId",
                principalSchema: "ProjectManagement",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHandoffs_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectHandoffs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaterialRequirements_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectMaterialRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskLinks_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectTaskLinks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskLinks_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectTaskLinks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaterialRequirements_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectMaterialRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ProjectHandoffs_ProjectId",
                schema: "ProjectManagement",
                table: "ProjectHandoffs");
        }
    }
}
