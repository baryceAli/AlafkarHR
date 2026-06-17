using Microsoft.EntityFrameworkCore.Design;

namespace Cart.Data;

public class CartDbContextFactory : IDesignTimeDbContextFactory<CartDbContext>
{
    public CartDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CartDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AlAfkarERPDesignTime;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        return new CartDbContext(options);
    }
}
