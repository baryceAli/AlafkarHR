using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrainingFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Training");

            migrationBuilder.CreateTable(
                name: "TrainingAttendees",
                schema: "Training",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attended = table.Column<bool>(type: "bit", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CertificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CertificateIssuer = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CertificateIssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificateExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_TrainingAttendees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingEvents",
                schema: "Training",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_TrainingEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPrograms",
                schema: "Training",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Objective = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_TrainingPrograms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAttendees_TrainingEventId_EmployeeId",
                schema: "Training",
                table: "TrainingAttendees",
                columns: new[] { "TrainingEventId", "EmployeeId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEvents_CompanyId_ProgramId_Status_StartAt",
                schema: "Training",
                table: "TrainingEvents",
                columns: new[] { "CompanyId", "ProgramId", "Status", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_CompanyId_Name",
                schema: "Training",
                table: "TrainingPrograms",
                columns: new[] { "CompanyId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingAttendees",
                schema: "Training");

            migrationBuilder.DropTable(
                name: "TrainingEvents",
                schema: "Training");

            migrationBuilder.DropTable(
                name: "TrainingPrograms",
                schema: "Training");
        }
    }
}
