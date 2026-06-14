using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    [DbContext(typeof(AttendanceDbContext))]
    [Migration("20260615000100_AddMissingAttendanceHolidayFields")]
    public partial class AddMissingAttendanceHolidayFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'Attendance.AttendanceHolidays', N'HolidayType') IS NULL
                BEGIN
                    ALTER TABLE [Attendance].[AttendanceHolidays]
                    ADD [HolidayType] int NOT NULL
                        CONSTRAINT [DF_AttendanceHolidays_HolidayType] DEFAULT(0);
                END

                IF COL_LENGTH(N'Attendance.AttendanceHolidays', N'IsRecurringYearly') IS NULL
                BEGIN
                    ALTER TABLE [Attendance].[AttendanceHolidays]
                    ADD [IsRecurringYearly] bit NOT NULL
                        CONSTRAINT [DF_AttendanceHolidays_IsRecurringYearly] DEFAULT(0);
                END

                IF COL_LENGTH(N'Attendance.AttendanceHolidays', N'IsActive') IS NULL
                BEGIN
                    ALTER TABLE [Attendance].[AttendanceHolidays]
                    ADD [IsActive] bit NOT NULL
                        CONSTRAINT [DF_AttendanceHolidays_IsActive] DEFAULT(1);
                END

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_AttendanceHolidays_CompanyId_IsActive'
                        AND object_id = OBJECT_ID(N'Attendance.AttendanceHolidays'))
                BEGIN
                    CREATE INDEX [IX_AttendanceHolidays_CompanyId_IsActive]
                    ON [Attendance].[AttendanceHolidays] ([CompanyId], [IsActive]);
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
