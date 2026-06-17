using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class TaskActionsAndDailyCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskActions",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedCompletionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
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
                    table.PrimaryKey("PK_TaskActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskActions_TaskItems_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskDailyCheckRuns",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRetryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_TaskDailyCheckRuns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_CreatedByUserId_TaskId",
                schema: "TaskManagement",
                table: "TaskActions",
                columns: new[] { "CreatedByUserId", "TaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_ExpectedCompletionAt",
                schema: "TaskManagement",
                table: "TaskActions",
                column: "ExpectedCompletionAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActions_TaskId",
                schema: "TaskManagement",
                table: "TaskActions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDailyCheckRuns_NextRetryAt",
                schema: "TaskManagement",
                table: "TaskDailyCheckRuns",
                column: "NextRetryAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDailyCheckRuns_UserId_CheckDate",
                schema: "TaskManagement",
                table: "TaskDailyCheckRuns",
                columns: new[] { "UserId", "CheckDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskActions",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskDailyCheckRuns",
                schema: "TaskManagement");
        }
    }
}
