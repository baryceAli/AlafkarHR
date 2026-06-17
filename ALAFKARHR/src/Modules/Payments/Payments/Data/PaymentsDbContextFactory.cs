using Microsoft.EntityFrameworkCore.Design;

namespace Payments.Data;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AlAfkarERPDesignTime;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        return new PaymentsDbContext(options);
    }
}
