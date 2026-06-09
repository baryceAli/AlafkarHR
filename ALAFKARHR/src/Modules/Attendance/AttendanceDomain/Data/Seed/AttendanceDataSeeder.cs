using AttendanceDomain.Attendance.Models;
using Shared.Data.Seed;

namespace AttendanceDomain.Data.Seed;

public class AttendanceDataSeeder : IDataSeeder<AttendanceDbContext>
{
    public async Task SeedAllAsync(AttendanceDbContext context)
    {
        if (await context.Shifts.AnyAsync())
        {
            return;
        }

        context.Shifts.Add(Shift.Create(
            Guid.Parse("da0d822c-95ec-45b6-9180-d4f99e97f243"),
            "Default Day Shift",
            new TimeSpan(8, 0, 0),
            new TimeSpan(17, 0, 0),
            15,
            15,
            120,
            60,
            Guid.Parse("11111111-1111-1111-1111-111111111111")));

        await context.SaveChangesAsync();
    }
}
