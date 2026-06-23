using System;
using System.Collections.Generic;
using System.Linq;
using Accounting.Accounting.Features;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

var companyId = Guid.NewGuid();
var settings = new AccountCodingSettingsDto { CompanyId = companyId };

AssertEqual("1000", AccountCodePattern.RootCode(settings, AccountType.Asset), "asset root");
AssertEqual("1001", AccountCodePattern.RootCode(settings, AccountType.Liability), "liability root");
AssertEqual("1002", AccountCodePattern.RootCode(settings, AccountType.Equity), "equity root");
AssertEqual("1003", AccountCodePattern.RootCode(settings, AccountType.Revenue), "revenue root");
AssertEqual("1004", AccountCodePattern.RootCode(settings, AccountType.Expense), "expense root");

var saudiTemplateCodes = AccountCodePattern.GenerateTemplateCodes(SaudiAccountingTemplate.Template, settings);
foreach (var account in SaudiAccountingTemplate.Template.Accounts)
    AssertEqual(saudiTemplateCodes[account.TemplateKey], account.Code, $"Saudi SME template account {account.TemplateKey}");

AssertEqual("1000", saudiTemplateCodes["SA_ASSETS"], "Saudi SME assets root");
AssertEqual("100001", saudiTemplateCodes["SA_CURRENT_ASSETS"], "Saudi SME current assets group");
AssertEqual("100002", saudiTemplateCodes["SA_FIXED_ASSETS"], "Saudi SME fixed assets group");
AssertEqual("100001001", saudiTemplateCodes["SA_CASH"], "Saudi SME cash ledger");
AssertEqual("100001002", saudiTemplateCodes["SA_BANK"], "Saudi SME bank ledger");
AssertEqual("100001003", saudiTemplateCodes["SA_RECEIVABLE"], "Saudi SME receivable ledger");
AssertEqual("1001", saudiTemplateCodes["SA_LIABILITIES"], "Saudi SME liabilities root");
AssertEqual("100101", saudiTemplateCodes["SA_CURRENT_LIABILITIES"], "Saudi SME current liabilities group");
AssertEqual("100101001", saudiTemplateCodes["SA_PAYABLE"], "Saudi SME payable ledger");
AssertEqual("100101002", saudiTemplateCodes["SA_OUTPUT_VAT"], "Saudi SME output VAT ledger");
AssertEqual("1000001", saudiTemplateCodes["SA_SUSPENSE"], "Saudi SME root-level asset suspense ledger");

var template = new AccountingTemplateDto
{
    Code = "PATTERN_TEST",
    Name = "Pattern Test",
    NameAr = "Pattern Test",
    CountryCode = "SA",
    CurrencyCode = "SAR",
    Accounts =
    [
        Group("ASSETS", "1000", AccountType.Asset),
        Group("ASSET_GROUP_1", "100001", AccountType.Asset, "ASSETS"),
        Group("ASSET_GROUP_2", "100002", AccountType.Asset, "ASSETS"),
        Ledger("ASSET_LEDGER_1", "1000001", AccountType.Asset, "ASSETS"),
        Ledger("ASSET_LEDGER_2", "1000002", AccountType.Asset, "ASSETS"),
        Group("ASSET_GROUP_1_CHILD_1", "10000101", AccountType.Asset, "ASSET_GROUP_1"),
        Group("ASSET_GROUP_1_CHILD_2", "10000102", AccountType.Asset, "ASSET_GROUP_1"),
        Ledger("ASSET_GROUP_1_LEDGER_1", "100001001", AccountType.Asset, "ASSET_GROUP_1"),
        Ledger("ASSET_GROUP_1_LEDGER_2", "100001002", AccountType.Asset, "ASSET_GROUP_1")
    ]
};

var generated = AccountCodePattern.GenerateTemplateCodes(template, settings);
AssertEqual("1000", generated["ASSETS"], "template asset root");
AssertEqual("100001", generated["ASSET_GROUP_1"], "first group under assets");
AssertEqual("100002", generated["ASSET_GROUP_2"], "second group under assets");
AssertEqual("1000001", generated["ASSET_LEDGER_1"], "first ledger under assets");
AssertEqual("1000002", generated["ASSET_LEDGER_2"], "second ledger under assets");
AssertEqual("10000101", generated["ASSET_GROUP_1_CHILD_1"], "first nested group under first asset group");
AssertEqual("10000102", generated["ASSET_GROUP_1_CHILD_2"], "second nested group under first asset group");
AssertEqual("100001001", generated["ASSET_GROUP_1_LEDGER_1"], "first nested ledger under first asset group");
AssertEqual("100001002", generated["ASSET_GROUP_1_LEDGER_2"], "second nested ledger under first asset group");

var assetRoot = Guid.NewGuid();
var group1 = Guid.NewGuid();
var group2 = Guid.NewGuid();
var ledger1 = Guid.NewGuid();
var ledger2 = Guid.NewGuid();
var nestedGroup1 = Guid.NewGuid();
var nestedGroup2 = Guid.NewGuid();
var nestedLedger1 = Guid.NewGuid();
var nestedLedger2 = Guid.NewGuid();

var planned = AccountCodePattern.PlanRenumberCodes(
[
    Node(assetRoot, null, "1000", AccountType.Asset, false),
    Node(group1, assetRoot, "100001", AccountType.Asset, false),
    Node(group2, assetRoot, "100002", AccountType.Asset, false),
    Node(ledger1, assetRoot, "1000001", AccountType.Asset, true),
    Node(ledger2, assetRoot, "1000002", AccountType.Asset, true),
    Node(nestedGroup1, group1, "10000101", AccountType.Asset, false),
    Node(nestedGroup2, group1, "10000102", AccountType.Asset, false),
    Node(nestedLedger1, group1, "100001001", AccountType.Asset, true),
    Node(nestedLedger2, group1, "100001002", AccountType.Asset, true)
], settings);

AssertEqual("1000", planned[assetRoot], "planned asset root");
AssertEqual("100001", planned[group1], "planned first group under assets");
AssertEqual("100002", planned[group2], "planned second group under assets");
AssertEqual("1000001", planned[ledger1], "planned first ledger under assets");
AssertEqual("1000002", planned[ledger2], "planned second ledger under assets");
AssertEqual("10000101", planned[nestedGroup1], "planned first nested group");
AssertEqual("10000102", planned[nestedGroup2], "planned second nested group");
AssertEqual("100001001", planned[nestedLedger1], "planned first nested ledger");
AssertEqual("100001002", planned[nestedLedger2], "planned second nested ledger");

var changedSuffixSettings = new AccountCodingSettingsDto
{
    CompanyId = companyId,
    ChildGroupSuffixLength = 3,
    ChildLedgerSuffixLength = 4
};
var suffixPlan = AccountCodePattern.PlanRenumberCodes(
[
    Node(assetRoot, null, "1000", AccountType.Asset, false),
    Node(group1, assetRoot, "100001", AccountType.Asset, false),
    Node(ledger1, group1, "100001001", AccountType.Asset, true)
], changedSuffixSettings);

AssertEqual("1000001", suffixPlan[group1], "suffix-renumbered group");
AssertEqual("10000010001", suffixPlan[ledger1], "suffix-renumbered ledger");

var directLedger = Guid.NewGuid();
var collisionNodes = new[]
{
    Node(assetRoot, null, "1000", AccountType.Asset, false),
    Node(group1, assetRoot, "100001", AccountType.Asset, false),
    Node(directLedger, assetRoot, "1000001", AccountType.Asset, true)
};
var collisionPlan = AccountCodePattern.PlanRenumberCodes(collisionNodes, changedSuffixSettings);
AssertEqual("1000001", collisionPlan[group1], "suffix-renumbered group collides with direct ledger old code");
AssertEqual("10000001", collisionPlan[directLedger], "suffix-renumbered direct ledger");
AssertTrue(HasIntermediateCollision(collisionNodes, collisionPlan), "suffix renumber has an intermediate unique-index collision without a temporary rename phase");

Console.WriteLine("Account coding pattern checks passed.");

static AccountingTemplateAccountDto Group(string key, string code, AccountType type, string? parentKey = null) => new()
{
    TemplateKey = key,
    Code = code,
    Name = key,
    NameEng = key,
    Type = type,
    NormalBalance = type == AccountType.Asset || type == AccountType.Expense ? NormalBalance.Debit : NormalBalance.Credit,
    ParentTemplateKey = parentKey,
    IsPostingAccount = false
};

static AccountingTemplateAccountDto Ledger(string key, string code, AccountType type, string parentKey) => new()
{
    TemplateKey = key,
    Code = code,
    Name = key,
    NameEng = key,
    Type = type,
    NormalBalance = type == AccountType.Asset || type == AccountType.Expense ? NormalBalance.Debit : NormalBalance.Credit,
    ParentTemplateKey = parentKey,
    IsPostingAccount = true
};

static AccountCodePattern.AccountNode Node(Guid id, Guid? parentId, string code, AccountType type, bool isPostingAccount) =>
    new(id, parentId, code, type, isPostingAccount);

static bool HasIntermediateCollision(IEnumerable<AccountCodePattern.AccountNode> nodes, IReadOnlyDictionary<Guid, string> plannedCodes)
{
    var oldCodesById = nodes.ToDictionary(x => x.Id, x => x.Code);
    foreach (var plannedCode in plannedCodes)
    {
        if (string.Equals(oldCodesById[plannedCode.Key], plannedCode.Value, StringComparison.Ordinal))
            continue;

        if (oldCodesById.Any(x => x.Key != plannedCode.Key && string.Equals(x.Value, plannedCode.Value, StringComparison.Ordinal)))
            return true;
    }

    return false;
}

static void AssertEqual(string expected, string actual, string scenario)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
        throw new InvalidOperationException($"{scenario}: expected '{expected}', got '{actual}'.");
}

static void AssertTrue(bool condition, string scenario)
{
    if (!condition)
        throw new InvalidOperationException($"{scenario}: expected condition to be true.");
}
