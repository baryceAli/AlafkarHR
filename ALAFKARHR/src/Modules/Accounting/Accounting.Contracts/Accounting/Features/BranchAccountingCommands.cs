using Shared.Contracts.CQRS;

namespace Accounting.Contracts.Accounting.Features;

public record EnsureBranchAccountingCommand(
    Guid CompanyId,
    Guid BranchId,
    string BranchCode,
    string BranchName,
    string BranchNameEng) : ICommand<EnsureBranchAccountingResult>;

public record EnsureBranchAccountingResult(int AccountGroupsCreated, int JournalsCreated);

