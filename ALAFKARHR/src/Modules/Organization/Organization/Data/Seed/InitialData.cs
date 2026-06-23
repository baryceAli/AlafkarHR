namespace Organization.Data.Seed;

public class InitialData
{
    public static Guid CateringBusinessLineId => Guid.Parse("544d599e-8d7d-4b58-bb4a-8bd9332d2bc5");
    public static Guid RealEstateBusinessLineId => Guid.Parse("7fd5d0df-2e43-40f4-bcb7-7ed625c925e6");
    public static Guid StoreFrontBusinessLineId => Guid.Parse("d3c85fd2-0fa8-4d7d-9f97-2c54c5f67b58");
    public static Guid BasicLicenseCategoryId => Guid.Parse("8ae8997e-42a6-4b39-8d35-5c86fdf2f031");
    public static Guid StandardLicenseCategoryId => Guid.Parse("65f1a088-c089-4681-a205-5f830da2a0c9");
    public static Guid ProLicenseCategoryId => Guid.Parse("96a7b174-932b-4ef9-b680-6ad0d1deea75");
    public static Guid AdvancedLicenseCategoryId => Guid.Parse("861b0e2e-5f63-427b-903f-1d20e9271fbd");
    public static Guid SarCurrencyId => Guid.Parse("71077eb6-cb32-49b1-bb41-72c1c7eeac5c");

    public static IEnumerable<BusinessLine> BusinessLines => new List<BusinessLine>
    {
        BusinessLine.Create(
            CateringBusinessLineId,
            SharedWithUI.Organization.BusinessLineKeys.Catering,
            "Catering",
            "خدمات الإعاشة",
            "bi-cup-hot",
            "Catering contracts, meals, locations, schedules, deliveries, assignments, and reports.",
            10,
            SharedWithUI.Organization.Enums.BusinessLineActivationPolicy.SinglePerCompany,
            "local-seed"),
        BusinessLine.Create(
            RealEstateBusinessLineId,
            SharedWithUI.Organization.BusinessLineKeys.RealEstate,
            "Real Estate",
            "العقارات",
            "bi-buildings",
            "Properties, units, leases, rent collections, utilities, expenses, and reports.",
            20,
            SharedWithUI.Organization.Enums.BusinessLineActivationPolicy.SinglePerCompany,
            "local-seed"),
        BusinessLine.Create(
            StoreFrontBusinessLineId,
            SharedWithUI.Organization.BusinessLineKeys.StoreFront,
            "Store Front",
            "واجهة المتجر",
            "bi-shop",
            "Stores, shop types, store-specific sellable goods and services, and store-aware POS.",
            30,
            SharedWithUI.Organization.Enums.BusinessLineActivationPolicy.MultiplePerCompany,
            "local-seed")
    };

    public static IEnumerable<Guid> ImplementedBusinessLineIds => new[]
    {
        CateringBusinessLineId,
        RealEstateBusinessLineId,
        StoreFrontBusinessLineId
    };

    public static IEnumerable<LicenseCategory> LicenseCategories => new List<LicenseCategory>
    {
        LicenseCategory.Create(
            BasicLicenseCategoryId,
            "basic",
            "Basic",
            5,
            1,
            1,
            50,
            500,
            SarCurrencyId,
            "SAR",
            "Default Basic license category.",
            "local-seed"),
        LicenseCategory.Create(
            StandardLicenseCategoryId,
            "standard",
            "Standard",
            25,
            5,
            10,
            100,
            1000,
            SarCurrencyId,
            "SAR",
            "Default Standard license category.",
            "local-seed"),
        LicenseCategory.Create(
            ProLicenseCategoryId,
            "pro",
            "Pro",
            50,
            10,
            25,
            200,
            2000,
            SarCurrencyId,
            "SAR",
            "Default Pro license category.",
            "local-seed"),
        LicenseCategory.Create(
            AdvancedLicenseCategoryId,
            "advanced",
            "Advanced",
            100,
            25,
            50,
            400,
            4000,
            SarCurrencyId,
            "SAR",
            "Default Advanced license category.",
            "local-seed")
    };

    public static Company Company => Company.Create(
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        null,
                        "ALAFKAR",
                        "ALAFKAR",
                        "logo",
                        "MAKKA",
                        10.1,
                        10.2,
                        "111111111111111",
                        "Afkr",
                        Guid.Parse("71077eb6-cb32-49b1-bb41-72c1c7eeac5c"),
                        "info@alafkar.com",
                        "0511111111",
                        "ksa",
                        "Baryce@gmail.com");
    public static IEnumerable<Branch> Branches => new List<Branch>()
    {
        Branch.Create(
                        Guid.Parse("4f825206-5179-413a-bbff-02bb047bab64"),
                        "Main Branch",
                        "Main Branch",
                        "Makka",
                        12.1,
                        12.1,
                        "br",
                        "05123456789",
                        "info@mainbranch.com",
                        true,
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        "baryce@gmail.com"
        ),
        Branch.Create(
                        Guid.Parse("3fe659f3-7f1c-4f02-b198-1c9cde4b86de"),
                        "Second Branch",
                        "Second Branch",
                        "Makka",
                        12.1,
                        12.1,
                        "br",
                        "05123456789",
                        "info@secondbranch.com",
                        false,
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        "baryce@gmail.com"
        )
};
}
