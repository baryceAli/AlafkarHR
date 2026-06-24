using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeHrCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Grade",
                schema: "Employee",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LinkedUserId",
                schema: "Employee",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerEmployeeId",
                schema: "Employee",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkLocation",
                schema: "Employee",
                table: "Employees",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeCertifications",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeCertifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocumentLinks",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeDocumentLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEmergencyContacts",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeEmergencyContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProficiencyLevel = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeSkills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HrLifecycleEvents",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromManagerEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToManagerEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromGrade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToGrade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FromWorkLocation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ToWorkLocation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HrLifecycleEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LinkedUserId",
                schema: "Employee",
                table: "Employees",
                column: "LinkedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerEmployeeId",
                schema: "Employee",
                table: "Employees",
                column: "ManagerEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertifications_CompanyId_EmployeeId_Name",
                schema: "Employee",
                table: "EmployeeCertifications",
                columns: new[] { "CompanyId", "EmployeeId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertifications_DocumentId",
                schema: "Employee",
                table: "EmployeeCertifications",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertifications_ExpiresAt",
                schema: "Employee",
                table: "EmployeeCertifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentLinks_CompanyId_EmployeeId_DocumentType",
                schema: "Employee",
                table: "EmployeeDocumentLinks",
                columns: new[] { "CompanyId", "EmployeeId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentLinks_DocumentId",
                schema: "Employee",
                table: "EmployeeDocumentLinks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentLinks_ExpiryDate",
                schema: "Employee",
                table: "EmployeeDocumentLinks",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEmergencyContacts_CompanyId_EmployeeId_IsPrimary",
                schema: "Employee",
                table: "EmployeeEmergencyContacts",
                columns: new[] { "CompanyId", "EmployeeId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_CompanyId_EmployeeId_SkillName",
                schema: "Employee",
                table: "EmployeeSkills",
                columns: new[] { "CompanyId", "EmployeeId", "SkillName" });

            migrationBuilder.CreateIndex(
                name: "IX_HrLifecycleEvents_CompanyId_EmployeeId_Status",
                schema: "Employee",
                table: "HrLifecycleEvents",
                columns: new[] { "CompanyId", "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HrLifecycleEvents_EffectiveDate",
                schema: "Employee",
                table: "HrLifecycleEvents",
                column: "EffectiveDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeCertifications",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeDocumentLinks",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeEmergencyContacts",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeSkills",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "HrLifecycleEvents",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LinkedUserId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ManagerEmployeeId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Grade",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LinkedUserId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ManagerEmployeeId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WorkLocation",
                schema: "Employee",
                table: "Employees");
        }
    }
}
