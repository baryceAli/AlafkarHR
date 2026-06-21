using GeneralSettings.GeneralSettings.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GeneralSettings.Data;
// add-migration SettingsInitial -Project GeneralSettings -StartupProject Api -OutputDir Data/Migrations -Context GeneralSettingsDbContext
// update-database -Project GeneralSettings -StartupProject Api -Context GeneralSettingsDbContext

public class GeneralSettingsDbContext:DbContext
{
    public GeneralSettingsDbContext(DbContextOptions<GeneralSettingsDbContext> options):base(options)
    {
        
    }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<CompanySetting> CompanySettings => Set<CompanySetting>();
    public DbSet<HomePageTemplateSelection> HomePageTemplateSelections => Set<HomePageTemplateSelection>();
    public DbSet<CurrentStorefrontHomePageContent> CurrentStorefrontHomePageContents => Set<CurrentStorefrontHomePageContent>();
    public DbSet<CorporateShowcaseHomePageContent> CorporateShowcaseHomePageContents => Set<CorporateShowcaseHomePageContent>();
    public DbSet<ProductHighlightHomePageContent> ProductHighlightHomePageContents => Set<ProductHighlightHomePageContent>();
    public DbSet<CampaignLandingHomePageContent> CampaignLandingHomePageContents => Set<CampaignLandingHomePageContent>();
    public DbSet<MinimalCatalogHomePageContent> MinimalCatalogHomePageContents => Set<MinimalCatalogHomePageContent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("GeneralSettings");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ConfigureHomePageTemplates(modelBuilder);

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);

    }

    private static void ConfigureHomePageTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HomePageTemplateSelection>(builder =>
        {
            builder.ToTable("HomePageTemplateSelections");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActiveTemplateKey).HasMaxLength(80).IsRequired();
            builder.HasIndex(x => x.CompanyId).IsUnique();
        });

        modelBuilder.Entity<HomePageTemplateContent>(builder =>
        {
            builder.UseTpcMappingStrategy();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SectionKey).HasMaxLength(80).IsRequired();
            builder.Property(x => x.FieldKey).HasMaxLength(80).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(20).IsRequired();
            builder.Property(x => x.TextEn).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.TextAr).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImagePath).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.AltTextEn).HasMaxLength(300).IsRequired();
            builder.Property(x => x.AltTextAr).HasMaxLength(300).IsRequired();
        });
        ConfigureHomePageContent<CurrentStorefrontHomePageContent>(modelBuilder, "CurrentStorefrontHomePageContents");
        ConfigureHomePageContent<CorporateShowcaseHomePageContent>(modelBuilder, "CorporateShowcaseHomePageContents");
        ConfigureHomePageContent<ProductHighlightHomePageContent>(modelBuilder, "ProductHighlightHomePageContents");
        ConfigureHomePageContent<CampaignLandingHomePageContent>(modelBuilder, "CampaignLandingHomePageContents");
        ConfigureHomePageContent<MinimalCatalogHomePageContent>(modelBuilder, "MinimalCatalogHomePageContents");
    }

    private static void ConfigureHomePageContent<TEntity>(ModelBuilder modelBuilder, string tableName)
        where TEntity : HomePageTemplateContent
    {
        modelBuilder.Entity<TEntity>(builder =>
        {
            builder.ToTable(tableName);
            builder.HasIndex(x => new { x.CompanyId, x.SectionKey, x.FieldKey }).IsUnique();
            builder.HasIndex(x => x.CompanyId);
        });
    }
}
