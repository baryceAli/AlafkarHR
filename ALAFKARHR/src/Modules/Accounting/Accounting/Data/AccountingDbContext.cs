using System.Reflection;

namespace Accounting.Data;

public class AccountingDbContext(DbContextOptions<AccountingDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    public DbSet<TaxCode> TaxCodes => Set<TaxCode>();
    public DbSet<PostingProfile> PostingProfiles => Set<PostingProfile>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<CashAccount> CashAccounts => Set<CashAccount>();
    public DbSet<CompanyAccountingSettings> CompanyAccountingSettings => Set<CompanyAccountingSettings>();
    public DbSet<AccountingTemplate> AccountingTemplates => Set<AccountingTemplate>();
    public DbSet<AccountingJournal> AccountingJournals => Set<AccountingJournal>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<AccountingDocument> AccountingDocuments => Set<AccountingDocument>();
    public DbSet<ZatcaSettings> ZatcaSettings => Set<ZatcaSettings>();
    public DbSet<ZatcaDevice> ZatcaDevices => Set<ZatcaDevice>();
    public DbSet<EInvoice> EInvoices => Set<EInvoice>();
    public DbSet<EInvoiceSubmission> EInvoiceSubmissions => Set<EInvoiceSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Accounting");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
