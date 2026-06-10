
using Shared.Data.Seed;

namespace Payroll.Data.Seed;

public class PayrollDataSeeder : IDataSeeder<PayrollDbContext>
{


    public Task SeedAllAsync(PayrollDbContext context)
    {
        //if(!await context.salaries.anyasync())
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}
