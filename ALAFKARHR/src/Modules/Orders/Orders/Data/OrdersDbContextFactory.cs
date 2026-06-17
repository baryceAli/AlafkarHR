using Microsoft.EntityFrameworkCore.Design;

namespace Orders.Data;

public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AlAfkarERPDesignTime;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        return new OrdersDbContext(options);
    }
}
