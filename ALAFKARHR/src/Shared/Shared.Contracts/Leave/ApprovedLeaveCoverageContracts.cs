using Shared.Contracts.CQRS;

namespace Shared.Contracts.Leave;

public record GetApprovedLeaveCoverageQuery(
    Guid CompanyId,
    List<Guid> EmployeeIds,
    DateTime FromDate,
    DateTime ToDate)
    : IQuery<GetApprovedLeaveCoverageResult>;

public record GetApprovedLeaveCoverageResult(List<ApprovedLeaveCoverageDay> Days);

public record ApprovedLeaveCoverageDay(Guid EmployeeId, DateTime Date);
