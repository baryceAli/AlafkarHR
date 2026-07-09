using SalesOrder.Orders.Models;
using System.Reflection;

namespace SalesOrder.Data;
// add-migration SalesOrderInitial -Project SalesOrder -StartupProject Api -OutputDir Data/Migrations -Context SalesOrderDbContext
// update-database -Project SalesOrder -StartupProject Api -Context SalesOrderDbContext

public class SalesOrderDbContext:DbContext
{
    public SalesOrderDbContext(DbContextOptions<SalesOrderDbContext> options):base(options)
    {
        
    }

    public DbSet<Orders.Models.SalesOrder> SalesOrders => Set<Orders.Models.SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines=> Set<SalesOrderLine>();
    public DbSet<SalesQuotation> SalesQuotations => Set<SalesQuotation>();
    public DbSet<SalesQuotationLine> SalesQuotationLines => Set<SalesQuotationLine>();
    public DbSet<SalesDeliveryNote> SalesDeliveryNotes => Set<SalesDeliveryNote>();
    public DbSet<SalesDeliveryNoteLine> SalesDeliveryNoteLines => Set<SalesDeliveryNoteLine>();
    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();
    public DbSet<SalesReturnLine> SalesReturnLines => Set<SalesReturnLine>();
    public DbSet<SalesSettings> SalesSettings => Set<SalesSettings>();
    public DbSet<SalesQuotationTemplate> SalesQuotationTemplates => Set<SalesQuotationTemplate>();
    public DbSet<SalesQuotationTemplateLine> SalesQuotationTemplateLines => Set<SalesQuotationTemplateLine>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SalesOrder");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
