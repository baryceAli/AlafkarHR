using System.Reflection;

namespace Procurement.Data;

public class ProcurementDbContext(DbContextOptions<ProcurementDbContext> options) : DbContext(options)
{
    public DbSet<ProcurementDocument> ProcurementDocuments => Set<ProcurementDocument>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<RequestForQuotation> RequestsForQuotation => Set<RequestForQuotation>();
    public DbSet<SupplierQuotation> SupplierQuotations => Set<SupplierQuotation>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<SupplierInvoice> SupplierInvoices => Set<SupplierInvoice>();
    public DbSet<SupplierItem> SupplierItems => Set<SupplierItem>();
    public DbSet<VendorPricelist> VendorPricelists => Set<VendorPricelist>();
    public DbSet<ReorderingRule> ReorderingRules => Set<ReorderingRule>();
    public DbSet<ProcurementAgreement> ProcurementAgreements => Set<ProcurementAgreement>();
    public DbSet<ProcurementAgreementLine> ProcurementAgreementLines => Set<ProcurementAgreementLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Procurement");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
