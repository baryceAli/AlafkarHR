namespace Accounting.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.NormalBalance).HasConversion<int>();
        builder.Property(x => x.Role).HasConversion<int>();
        builder.Property(x => x.TemplateKey).HasMaxLength(80);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.TemplateKey });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class AccountingJournalConfiguration : IEntityTypeConfiguration<AccountingJournal>
{
    public void Configure(EntityTypeBuilder<AccountingJournal> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.ZatcaDeviceSerial).HasMaxLength(120);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.StartDate, x.EndDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class TaxCodeConfiguration : IEntityTypeConfiguration<TaxCode>
{
    public void Configure(EntityTypeBuilder<TaxCode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Rate).HasPrecision(9, 4);
        builder.Property(x => x.ZatcaCategoryCode).HasMaxLength(20);
        builder.Property(x => x.ExemptionReasonCode).HasMaxLength(40);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class PostingProfileConfiguration : IEntityTypeConfiguration<PostingProfile>
{
    public void Configure(EntityTypeBuilder<PostingProfile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.Type, x.IsDefault });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.BankName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.AccountNumber).HasMaxLength(80);
        builder.Property(x => x.Iban).HasMaxLength(80);
        builder.Property(x => x.BranchCode).HasMaxLength(40);
        builder.Property(x => x.Swift).HasMaxLength(40);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.DisplayName }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsDefault });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CashAccountConfiguration : IEntityTypeConfiguration<CashAccount>
{
    public void Configure(EntityTypeBuilder<CashAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.DisplayName }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsDefault });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CompanyAccountingSettingsConfiguration : IEntityTypeConfiguration<CompanyAccountingSettings>
{
    public void Configure(EntityTypeBuilder<CompanyAccountingSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CompanyId).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class AccountingTemplateConfiguration : IEntityTypeConfiguration<AccountingTemplate>
{
    public void Configure(EntityTypeBuilder<AccountingTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CountryCode).HasMaxLength(2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Visibility).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsMany(x => x.Accounts, line =>
        {
            line.ToTable("AccountingTemplateAccounts");
            line.WithOwner().HasForeignKey("AccountingTemplateId");
            line.HasKey("Id");
            line.Property(x => x.TemplateKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.Code).HasMaxLength(40).IsRequired();
            line.Property(x => x.Name).HasMaxLength(200).IsRequired();
            line.Property(x => x.NameEng).HasMaxLength(200).IsRequired();
            line.Property(x => x.Type).HasConversion<int>();
            line.Property(x => x.NormalBalance).HasConversion<int>();
            line.Property(x => x.Role).HasConversion<int>();
            line.Property(x => x.ParentTemplateKey).HasMaxLength(80);
            line.HasIndex("AccountingTemplateId", "TemplateKey").IsUnique();
            line.HasIndex("AccountingTemplateId", "Code").IsUnique();
        });
        builder.Navigation(x => x.Accounts).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.TaxCodes, line =>
        {
            line.ToTable("AccountingTemplateTaxCodes");
            line.WithOwner().HasForeignKey("AccountingTemplateId");
            line.HasKey("Id");
            line.Property(x => x.Code).HasMaxLength(30).IsRequired();
            line.Property(x => x.Name).HasMaxLength(160).IsRequired();
            line.Property(x => x.Rate).HasPrecision(9, 4);
            line.Property(x => x.ZatcaCategoryCode).HasMaxLength(20);
            line.Property(x => x.ExemptionReasonCode).HasMaxLength(40);
            line.HasIndex("AccountingTemplateId", "Code").IsUnique();
        });
        builder.Navigation(x => x.TaxCodes).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.PostingProfiles, line =>
        {
            line.ToTable("AccountingTemplatePostingProfiles");
            line.WithOwner().HasForeignKey("AccountingTemplateId");
            line.HasKey("Id");
            line.Property(x => x.Type).HasConversion<int>();
            line.Property(x => x.ReceivableAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.PayableAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.RevenueAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.ExpenseAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.OutputVatAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.InputVatAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.CashAccountKey).HasMaxLength(80).IsRequired();
            line.Property(x => x.BankAccountKey).HasMaxLength(80).IsRequired();
            line.HasIndex("AccountingTemplateId", "Type").IsUnique();
        });
        builder.Navigation(x => x.PostingProfiles).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Journals, line =>
        {
            line.ToTable("AccountingTemplateJournals");
            line.WithOwner().HasForeignKey("AccountingTemplateId");
            line.HasKey("Id");
            line.Property(x => x.Code).HasMaxLength(30).IsRequired();
            line.Property(x => x.Name).HasMaxLength(160).IsRequired();
            line.Property(x => x.NameAr).HasMaxLength(160).IsRequired();
            line.Property(x => x.Type).HasConversion<int>();
            line.Property(x => x.DefaultDebitAccountKey).HasMaxLength(80);
            line.Property(x => x.DefaultCreditAccountKey).HasMaxLength(80);
            line.HasIndex("AccountingTemplateId", "Code").IsUnique();
        });
        builder.Navigation(x => x.Journals).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.SourceModule).HasMaxLength(80);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(100);
        builder.Property(x => x.Memo).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.Number }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsMany(x => x.Lines, line =>
        {
            line.ToTable("JournalEntryLines");
            line.WithOwner().HasForeignKey("JournalEntryId");
            line.HasKey("Id");
            line.Property(x => x.Debit).HasPrecision(18, 2);
            line.Property(x => x.Credit).HasPrecision(18, 2);
            line.Property(x => x.Description).HasMaxLength(500);
            line.HasIndex("JournalEntryId", "LineNumber");
        });
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class AccountingDocumentConfiguration : IEntityTypeConfiguration<AccountingDocument>
{
    public void Configure(EntityTypeBuilder<AccountingDocument> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Number).HasMaxLength(80).IsRequired();
        builder.Property(x => x.PartyName).HasMaxLength(250);
        builder.Property(x => x.PartyVatNumber).HasMaxLength(30);
        builder.Property(x => x.SourceModule).HasMaxLength(80);
        builder.Property(x => x.SourceDocumentNumber).HasMaxLength(100);
        builder.Property(x => x.Subtotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.CompanyId, x.Type, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.SourceDocumentId });
        builder.HasIndex(x => new { x.CompanyId, x.Type, x.SourceModule, x.SourceDocumentId })
            .IsUnique()
            .HasFilter("[SourceModule] IS NOT NULL AND [SourceDocumentId] IS NOT NULL");
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsMany(x => x.Lines, line =>
        {
            line.ToTable("AccountingDocumentLines");
            line.WithOwner().HasForeignKey("AccountingDocumentId");
            line.HasKey("Id");
            line.Property(x => x.Description).HasMaxLength(500).IsRequired();
            line.Property(x => x.Quantity).HasPrecision(18, 4);
            line.Property(x => x.UnitPrice).HasPrecision(18, 4);
            line.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            line.Property(x => x.TaxRate).HasPrecision(9, 4);
            line.Property(x => x.TaxCode).HasMaxLength(30);
            line.Property(x => x.NetAmount).HasPrecision(18, 2);
            line.Property(x => x.TaxAmount).HasPrecision(18, 2);
            line.Property(x => x.TotalAmount).HasPrecision(18, 2);
            line.HasIndex("AccountingDocumentId", "LineNumber");
        });
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ZatcaSettingsConfiguration : IEntityTypeConfiguration<ZatcaSettings>
{
    public void Configure(EntityTypeBuilder<ZatcaSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SellerName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.SellerNameAr).HasMaxLength(250).IsRequired();
        builder.Property(x => x.VatNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CommercialRegistrationNumber).HasMaxLength(40);
        builder.Property(x => x.BuildingNumber).HasMaxLength(20);
        builder.Property(x => x.StreetName).HasMaxLength(120);
        builder.Property(x => x.District).HasMaxLength(120);
        builder.Property(x => x.City).HasMaxLength(120);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.CountryCode).HasMaxLength(2);
        builder.Property(x => x.Environment).HasMaxLength(40);
        builder.HasIndex(x => x.CompanyId).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ZatcaDeviceConfiguration : IEntityTypeConfiguration<ZatcaDevice>
{
    public void Configure(EntityTypeBuilder<ZatcaDevice> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Csid).HasMaxLength(500);
        builder.Property(x => x.PrivateKeyReference).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EInvoiceConfiguration : IEntityTypeConfiguration<EInvoice>
{
    public void Configure(EntityTypeBuilder<EInvoice> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.InvoiceType).HasConversion<int>();
        builder.Property(x => x.InvoiceHash).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PreviousInvoiceHash).HasMaxLength(200);
        builder.Property(x => x.SubmissionStatus).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.Icv }).IsUnique();
        builder.HasIndex(x => x.AccountingDocumentId).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EInvoiceSubmissionConfiguration : IEntityTypeConfiguration<EInvoiceSubmission>
{
    public void Configure(EntityTypeBuilder<EInvoiceSubmission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.HasIndex(x => new { x.EInvoiceId, x.CreatedAt });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
