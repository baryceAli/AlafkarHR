using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class TaskWorkflowExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextOccurrenceDate",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTaskId",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceEndType",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Never");

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceFrequency",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceInterval",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceMaxOccurrences",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceOccurrencesCreated",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderNotificationSentAt",
                schema: "TaskManagement",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_NextOccurrenceDate",
                schema: "TaskManagement",
                table: "TaskItems",
                column: "NextOccurrenceDate");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ParentTaskId",
                schema: "TaskManagement",
                table: "TaskItems",
                column: "ParentTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_NextOccurrenceDate",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_ParentTaskId",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "NextOccurrenceDate",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "ParentTaskId",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndType",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceFrequency",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceInterval",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceMaxOccurrences",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "RecurrenceOccurrencesCreated",
                schema: "TaskManagement",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "ReminderNotificationSentAt",
                schema: "TaskManagement",
                table: "TaskItems");
        }
    }
}
