-- Guarded cleanup for legacy orphan account:
-- Code: 11101
-- Name: الصندوق الرئيسي
--
-- This script soft-deletes only accounts that match the target identity and are
-- confirmed to be parentless and unreferenced by accounting setup/posting data.

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @TargetAccounts TABLE
(
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    CompanyId uniqueidentifier NOT NULL,
    BranchId uniqueidentifier NULL,
    Code nvarchar(40) NOT NULL,
    Name nvarchar(200) NOT NULL,
    NameEng nvarchar(200) NOT NULL,
    ParentAccountId uniqueidentifier NULL,
    IsPostingAccount bit NOT NULL,
    IsSystemAccount bit NOT NULL,
    IsActive bit NOT NULL,
    IsDeleted bit NOT NULL
);

INSERT INTO @TargetAccounts
(
    Id,
    CompanyId,
    BranchId,
    Code,
    Name,
    NameEng,
    ParentAccountId,
    IsPostingAccount,
    IsSystemAccount,
    IsActive,
    IsDeleted
)
SELECT
    Id,
    CompanyId,
    BranchId,
    Code,
    Name,
    NameEng,
    ParentAccountId,
    IsPostingAccount,
    IsSystemAccount,
    IsActive,
    IsDeleted
FROM Accounting.Accounts
WHERE
    (Code = N'11101' OR Name = N'الصندوق الرئيسي')
    AND IsDeleted = 0;

SELECT
    'TargetBefore' AS ResultSet,
    Id,
    CompanyId,
    BranchId,
    Code,
    Name,
    NameEng,
    ParentAccountId,
    IsPostingAccount,
    IsSystemAccount,
    IsActive,
    IsDeleted
FROM @TargetAccounts
ORDER BY CompanyId, Code, Name;

DECLARE @ReferenceCounts TABLE
(
    RefName nvarchar(80) NOT NULL,
    RefCount int NOT NULL
);

INSERT INTO @ReferenceCounts (RefName, RefCount)
SELECT N'NonParentlessTargets', COUNT(*)
FROM @TargetAccounts
WHERE ParentAccountId IS NOT NULL
UNION ALL
SELECT N'Children', COUNT(*)
FROM Accounting.Accounts
WHERE ParentAccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'CashAccounts', COUNT(*)
FROM Accounting.CashAccounts
WHERE LedgerAccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'BankAccounts', COUNT(*)
FROM Accounting.BankAccounts
WHERE LedgerAccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'CompanyAccountingSettings', COUNT(*)
FROM Accounting.CompanyAccountingSettings
WHERE
    ReceivableAccountId IN (SELECT Id FROM @TargetAccounts)
    OR PayableAccountId IN (SELECT Id FROM @TargetAccounts)
    OR RevenueAccountId IN (SELECT Id FROM @TargetAccounts)
    OR ExpenseAccountId IN (SELECT Id FROM @TargetAccounts)
    OR CogsAccountId IN (SELECT Id FROM @TargetAccounts)
    OR InventoryAccountId IN (SELECT Id FROM @TargetAccounts)
    OR InputVatAccountId IN (SELECT Id FROM @TargetAccounts)
    OR OutputVatAccountId IN (SELECT Id FROM @TargetAccounts)
    OR VatSettlementAccountId IN (SELECT Id FROM @TargetAccounts)
    OR CashAccountId IN (SELECT Id FROM @TargetAccounts)
    OR BankAccountId IN (SELECT Id FROM @TargetAccounts)
    OR RoundingAccountId IN (SELECT Id FROM @TargetAccounts)
    OR SuspenseAccountId IN (SELECT Id FROM @TargetAccounts)
    OR RetainedEarningsAccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'JournalDefaults', COUNT(*)
FROM Accounting.AccountingJournals
WHERE
    DefaultDebitAccountId IN (SELECT Id FROM @TargetAccounts)
    OR DefaultCreditAccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'AccountingDocumentLines', COUNT(*)
FROM Accounting.AccountingDocumentLines
WHERE AccountId IN (SELECT Id FROM @TargetAccounts)
UNION ALL
SELECT N'JournalEntryLines', COUNT(*)
FROM Accounting.JournalEntryLines
WHERE AccountId IN (SELECT Id FROM @TargetAccounts);

SELECT 'ReferenceCounts' AS ResultSet, RefName, RefCount
FROM @ReferenceCounts
ORDER BY RefName;

IF NOT EXISTS (SELECT 1 FROM @TargetAccounts)
BEGIN
    SELECT 'No matching active target account was found. No cleanup was needed.' AS CleanupResult;
    ROLLBACK TRANSACTION;
    RETURN;
END;

IF EXISTS (SELECT 1 FROM @ReferenceCounts WHERE RefCount > 0)
BEGIN
    SELECT 'Cleanup was blocked because the account is parented or referenced. No changes were made.' AS CleanupResult;
    ROLLBACK TRANSACTION;
    RETURN;
END;

UPDATE account
SET
    IsActive = 0,
    IsDeleted = 1,
    DeletedAt = SYSUTCDATETIME(),
    DeletedBy = N'guarded-account-cleanup',
    ModifiedAt = SYSUTCDATETIME(),
    ModifiedBy = N'guarded-account-cleanup'
FROM Accounting.Accounts AS account
INNER JOIN @TargetAccounts AS target ON target.Id = account.Id;

SELECT @@ROWCOUNT AS AccountsSoftDeleted;

SELECT
    'TargetAfter' AS ResultSet,
    Id,
    CompanyId,
    BranchId,
    Code,
    Name,
    NameEng,
    ParentAccountId,
    IsPostingAccount,
    IsSystemAccount,
    IsActive,
    IsDeleted,
    DeletedAt,
    DeletedBy
FROM Accounting.Accounts
WHERE Id IN (SELECT Id FROM @TargetAccounts)
ORDER BY CompanyId, Code, Name;

SELECT COUNT(*) AS ActiveChartMatchesAfterCleanup
FROM Accounting.Accounts
WHERE
    (Code = N'11101' OR Name = N'الصندوق الرئيسي')
    AND IsActive = 1
    AND IsDeleted = 0;

COMMIT TRANSACTION;
