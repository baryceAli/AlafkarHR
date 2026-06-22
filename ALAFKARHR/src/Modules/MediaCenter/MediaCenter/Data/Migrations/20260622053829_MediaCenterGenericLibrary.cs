using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaCenter.Data.Migrations
{
    /// <inheritdoc />
    public partial class MediaCenterGenericLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaActivityAllocations",
                schema: "MediaCenter");

            migrationBuilder.DropTable(
                name: "MediaActivityCustomers",
                schema: "MediaCenter");

            migrationBuilder.DropIndex(
                name: "IX_MediaActivities_DistributionPlaceId",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.DropIndex(
                name: "IX_MediaActivities_ProjectId",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.DropColumn(
                name: "DistributionPlaceId",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.DropColumn(
                name: "PlaceName",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                schema: "MediaCenter",
                table: "MediaActivities");

            migrationBuilder.RenameColumn(
                name: "FreeTextLocation",
                schema: "MediaCenter",
                table: "MediaActivities",
                newName: "LocationText");

            migrationBuilder.CreateTable(
                name: "MediaActivityRelatedRecords",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RelatedRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_MediaActivityRelatedRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaActivityRelatedRecords_MediaActivities_MediaActivityId",
                        column: x => x.MediaActivityId,
                        principalSchema: "MediaCenter",
                        principalTable: "MediaActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityRelatedRecords_MediaActivityId",
                schema: "MediaCenter",
                table: "MediaActivityRelatedRecords",
                column: "MediaActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityRelatedRecords_RelatedRecordId",
                schema: "MediaCenter",
                table: "MediaActivityRelatedRecords",
                column: "RelatedRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaActivityRelatedRecords_RelatedType",
                schema: "MediaCenter",
                table: "MediaActivityRelatedRecords",
                column: "RelatedType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaActivityRelatedRecords",
                schema: "MediaCenter");

            migrationBuilder.RenameColumn(
                name: "LocationText",
                schema: "MediaCenter",
                table: "MediaActivities",
                newName: "FreeTextLocation");

            migrationBuilder.AddColumn<Guid>(
                name: "DistributionPlaceId",
                schema: "MediaCenter",
                table: "MediaActivities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceName",
                schema: "MediaCenter",
                table: "MediaActivities",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                schema: "MediaCenter",
                table: "MediaActivities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                schema: "MediaCenter",
                table: "MediaActivities",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MediaActivityAllocations",
                schema: "MediaCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliverableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliverableName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DistributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistributionPlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProjectCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectDistributionAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CustomerNameEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MediaActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectCustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
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
        }
    }
}
