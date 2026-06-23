namespace Accounting.Accounting.Features;

public static class AccountCodePattern
{
    public sealed record AccountNode(Guid Id, Guid? ParentAccountId, string Code, AccountType Type, bool IsPostingAccount);

    public static string RootCode(AccountCodingSettingsDto settings, AccountType type) => type switch
    {
        AccountType.Asset => settings.AssetRootCode.Trim(),
        AccountType.Liability => settings.LiabilityRootCode.Trim(),
        AccountType.Equity => settings.EquityRootCode.Trim(),
        AccountType.Revenue => settings.RevenueRootCode.Trim(),
        AccountType.Expense => settings.ExpenseRootCode.Trim(),
        _ => throw new BadRequestException("Unsupported account type.")
    };

    public static bool RootCodesChanged(AccountCodingSettingsDto current, AccountCodingSettingsDto requested) =>
        !string.Equals(current.AssetRootCode?.Trim(), requested.AssetRootCode?.Trim(), StringComparison.OrdinalIgnoreCase)
        || !string.Equals(current.LiabilityRootCode?.Trim(), requested.LiabilityRootCode?.Trim(), StringComparison.OrdinalIgnoreCase)
        || !string.Equals(current.EquityRootCode?.Trim(), requested.EquityRootCode?.Trim(), StringComparison.OrdinalIgnoreCase)
        || !string.Equals(current.RevenueRootCode?.Trim(), requested.RevenueRootCode?.Trim(), StringComparison.OrdinalIgnoreCase)
        || !string.Equals(current.ExpenseRootCode?.Trim(), requested.ExpenseRootCode?.Trim(), StringComparison.OrdinalIgnoreCase);

    public static bool SuffixLengthsChanged(AccountCodingSettingsDto current, AccountCodingSettingsDto requested) =>
        current.ChildGroupSuffixLength != requested.ChildGroupSuffixLength
        || current.ChildLedgerSuffixLength != requested.ChildLedgerSuffixLength;

    public static bool StructuralCodingChanged(AccountCodingSettingsDto current, AccountCodingSettingsDto requested) =>
        RootCodesChanged(current, requested) || SuffixLengthsChanged(current, requested);

    public static Dictionary<string, string> GenerateTemplateCodes(AccountingTemplateDto template, AccountCodingSettingsDto settings)
    {
        var byKey = template.Accounts.ToDictionary(x => x.TemplateKey, StringComparer.OrdinalIgnoreCase);
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var counters = new Dictionary<(string ParentKey, bool Posting), int>();

        foreach (var account in template.Accounts.OrderBy(x => x.ParentTemplateKey is null ? 0 : 1).ThenBy(x => x.Code))
            Resolve(account.TemplateKey);

        return codes;

        string Resolve(string key)
        {
            if (codes.TryGetValue(key, out var existing))
                return existing;

            var account = byKey[key];
            if (string.IsNullOrWhiteSpace(account.ParentTemplateKey))
            {
                codes[key] = RootCode(settings, account.Type);
                return codes[key];
            }

            var parentCode = Resolve(account.ParentTemplateKey);
            var counterKey = (account.ParentTemplateKey!, account.IsPostingAccount);
            counters[counterKey] = counters.GetValueOrDefault(counterKey) + 1;
            var suffixLength = account.IsPostingAccount ? settings.ChildLedgerSuffixLength : settings.ChildGroupSuffixLength;
            codes[key] = NextChildCode(parentCode, counters[counterKey], suffixLength);
            return codes[key];
        }
    }

    public static Dictionary<Guid, string> PlanRenumberCodes(IReadOnlyList<AccountNode> accounts, AccountCodingSettingsDto requestedSettings)
    {
        var plannedCodes = accounts.ToDictionary(x => x.Id, x => x.Code);
        var childrenByParentId = accounts
            .Where(x => x.ParentAccountId.HasValue)
            .GroupBy(x => x.ParentAccountId!.Value)
            .ToDictionary(x => x.Key, x => x.OrderBy(account => account.Code).ToList());
        var roots = accounts
            .Where(x => !x.IsPostingAccount && !x.ParentAccountId.HasValue)
            .OrderBy(x => x.Code)
            .ToList();

        foreach (var root in roots)
        {
            plannedCodes[root.Id] = RootCode(requestedSettings, root.Type);
            PlanChildren(root.Id);
        }

        return plannedCodes;

        void PlanChildren(Guid parentId)
        {
            if (!childrenByParentId.TryGetValue(parentId, out var children))
                return;

            var groupCounter = 0;
            var ledgerCounter = 0;
            foreach (var child in children)
            {
                var suffixLength = child.IsPostingAccount ? requestedSettings.ChildLedgerSuffixLength : requestedSettings.ChildGroupSuffixLength;
                var sequence = child.IsPostingAccount ? ++ledgerCounter : ++groupCounter;
                plannedCodes[child.Id] = NextChildCode(plannedCodes[parentId], sequence, suffixLength);
                PlanChildren(child.Id);
            }
        }
    }

    private static string NextChildCode(string parentCode, int sequence, int suffixLength) =>
        $"{parentCode}{sequence.ToString($"D{suffixLength}")}";
}
