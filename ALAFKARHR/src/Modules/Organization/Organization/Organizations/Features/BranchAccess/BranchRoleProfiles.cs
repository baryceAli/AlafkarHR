namespace Organization.Organizations.Features.BranchAccess;

public static class BranchRoleProfiles
{
    public const string StoreAdmin = "store-admin";
    public const string StoreCashier = "store-cashier";
    public const string StoreAccountant = "store-accountant";
    public const string StoreWarehouse = "store-warehouse";

    private static readonly IReadOnlyList<BranchRoleProfileDto> Profiles =
    [
        new()
        {
            TemplateKey = StoreAdmin,
            Name = "Store Admin",
            NameAr = "مدير المتجر",
            Permissions =
            [
                PermissionList.StoreFrontStorePermissions.View,
                PermissionList.StoreFrontStorePermissions.Create,
                PermissionList.StoreFrontStorePermissions.Edit,
                PermissionList.StoreFrontStorePermissions.Delete,
                PermissionList.StoreFrontItemPermissions.View,
                PermissionList.StoreFrontItemPermissions.Edit,
                PermissionList.StoreFrontPosPermissions.View,
                PermissionList.StoreFrontPosPermissions.Checkout,
                PermissionList.StoreFrontPosPermissions.PriceOverride
            ]
        },
        new()
        {
            TemplateKey = StoreCashier,
            Name = "Store Cashier",
            NameAr = "كاشير المتجر",
            Permissions =
            [
                PermissionList.StoreFrontStorePermissions.View,
                PermissionList.StoreFrontPosPermissions.View,
                PermissionList.StoreFrontPosPermissions.Checkout
            ]
        },
        new()
        {
            TemplateKey = StoreAccountant,
            Name = "Store Accountant",
            NameAr = "محاسب المتجر",
            Permissions =
            [
                PermissionList.StoreFrontStorePermissions.View,
                PermissionList.StoreFrontPosPermissions.View,
                PermissionList.PaymentPermissions.View,
                PermissionList.PaymentPermissions.Create,
                PermissionList.AccountingDocumentPermissions.View,
                PermissionList.AccountingReportPermissions.View
            ]
        },
        new()
        {
            TemplateKey = StoreWarehouse,
            Name = "Store Warehouse",
            NameAr = "مستودع المتجر",
            Permissions =
            [
                PermissionList.StoreFrontStorePermissions.View,
                PermissionList.StoreFrontItemPermissions.View,
                PermissionList.WarehousePermissions.View,
                PermissionList.InventoryPermissions.View,
                PermissionList.StockTransactionPermissions.View,
                PermissionList.WarehouseTransferPermissions.View
            ]
        }
    ];

    public static IReadOnlyList<BranchRoleProfileDto> All => Profiles;

    public static BranchRoleProfileDto GetRequired(string templateKey)
        => Profiles.FirstOrDefault(profile => string.Equals(profile.TemplateKey, templateKey, StringComparison.OrdinalIgnoreCase))
           ?? throw new BadRequestException($"Unknown branch role profile: {templateKey}");

    public static bool HasPermission(string templateKey, string permission)
        => GetRequired(templateKey).Permissions.Contains(permission, StringComparer.Ordinal);
}
