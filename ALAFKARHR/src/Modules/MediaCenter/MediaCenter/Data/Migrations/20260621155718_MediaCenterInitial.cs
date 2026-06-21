using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaCenter.Data.Migrations
{
    /// <inheritdoc />
    public partial class MediaCenterInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MediaCenter");

            migrationBuilder.CreateTable(
                name: "MediaActivities",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TitleEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DistributionPlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaceName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FreeTextLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_MediaActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaActivityTypes",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_MediaActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaActivityAllocations",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectDistributionAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DeliverableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliverableName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DistributionPlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaceName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_MediaActivityAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaActivityAllocations_MediaActivities_MediaActivityId",
                        column: x => x.MediaActivityId,
                        principalSchema: "MediaCenter",
                        principalTable: "MediaActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaActivityCustomers",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CustomerNameEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProjectCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectCustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_MediaActivityCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaActivityCustomers_MediaActivities_MediaActivityId",
                        column: x => x.MediaActivityId,
                        principalSchema: "MediaCenter",
                        principalTable: "MediaActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaActivityMedia",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaKind = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_MediaActivityMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaActivityMedia_MediaActivities_MediaActivityId",
                        column: x => x.MediaActivityId,
                        principalSchema: "MediaCenter",
                        principalTable: "MediaActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivities_CompanyId_ActivityDate",
                schema: "MediaCenter",
                table: "MediaActivities",
                columns: new[] { "CompanyId", "ActivityDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivities_CompanyId_ActivityTypeId",
                schema: "MediaCenter",
                table: "MediaActivities",
                columns: new[] { "CompanyId", "ActivityTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivities_DistributionPlaceId",
                schema: "MediaCenter",
                table: "MediaActivities",
                column: "DistributionPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivities_ProjectId",
                schema: "MediaCenter",
                table: "MediaActivities",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityAllocations_DistributionDate_ProjectCustomerId_DistributionPlaceId",
                schema: "MediaCenter",
                table: "MediaActivityAllocations",
                columns: new[] { "DistributionDate", "ProjectCustomerId", "DistributionPlaceId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityAllocations_MediaActivityId",
                schema: "MediaCenter",
                table: "MediaActivityAllocations",
                column: "MediaActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityAllocations_ProjectDistributionAllocationId",
                schema: "MediaCenter",
                table: "MediaActivityAllocations",
                column: "ProjectDistributionAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityCustomers_CustomerId",
                schema: "MediaCenter",
                table: "MediaActivityCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityCustomers_MediaActivityId",
                schema: "MediaCenter",
                table: "MediaActivityCustomers",
                column: "MediaActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityCustomers_ProjectCustomerId",
                schema: "MediaCenter",
                table: "MediaActivityCustomers",
                column: "ProjectCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityMedia_DocumentId",
                schema: "MediaCenter",
                table: "MediaActivityMedia",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityMedia_MediaActivityId",
                schema: "MediaCenter",
                table: "MediaActivityMedia",
                column: "MediaActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityMedia_MediaKind",
                schema: "MediaCenter",
                table: "MediaActivityMedia",
                column: "MediaKind");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityTypes_CompanyId_Name",
                schema: "MediaCenter",
                table: "MediaActivityTypes",
                columns: new[] { "CompanyId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaActivityAllocations",
                schema: "MediaCenter");

            migrationBuilder.DropTable(
                name: "MediaActivityCustomers",
                schema: "MediaCenter");

            migrationBuilder.DropTable(
                name: "MediaActivityMedia",
                schema: "MediaCenter");

            migrationBuilder.DropTable(
                name: "MediaActivityTypes",
                schema: "MediaCenter");

            migrationBuilder.DropTable(
                name: "MediaActivities",
                schema: "MediaCenter");
        }
    }
}
