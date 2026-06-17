using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.BreakPolicies;

public record GetAttendanceBreakPoliciesQuery(Guid CompanyId) : IQuery<GetAttendanceBreakPoliciesResult>;
public record GetAttendanceBreakPoliciesResult(List<AttendanceBreakPolicyDto> PolicyList);
public record UpsertAttendanceBreakPolicyCommand(UpsertAttendanceBreakPolicyDto Policy, string? ModifiedBy)
    : ICommand<UpsertAttendanceBreakPolicyResult>;
public record UpsertAttendanceBreakPolicyResult(AttendanceBreakPolicyDto Policy);

public class GetAttendanceBreakPoliciesHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceBreakPoliciesQuery, GetAttendanceBreakPoliciesResult>
{
    public async Task<GetAttendanceBreakPoliciesResult> Handle(GetAttendanceBreakPoliciesQuery request, CancellationToken cancellationToken)
    {
        var policies = await dbContext.AttendanceBreakPolicies
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderByDescending(x =>
                x.Scope == ShiftAssignmentScope.Employee ? 4 :
                x.Scope == ShiftAssignmentScope.Department ? 3 :
                x.Scope == ShiftAssignmentScope.Administration ? 2 :
                x.Scope == ShiftAssignmentScope.Company ? 1 : 0)
            .ProjectToType<AttendanceBreakPolicyDto>()
            .ToListAsync(cancellationToken);

        return new GetAttendanceBreakPoliciesResult(policies);
    }
}

public class UpsertAttendanceBreakPolicyHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertAttendanceBreakPolicyCommand, UpsertAttendanceBreakPolicyResult>
{
    public async Task<UpsertAttendanceBreakPolicyResult> Handle(UpsertAttendanceBreakPolicyCommand request, CancellationToken cancellationToken)
    {
        AttendanceBreakPolicy policy;
        if (request.Policy.Id.HasValue && request.Policy.Id.Value != Guid.Empty)
        {
            policy = await dbContext.AttendanceBreakPolicies
                .FirstOrDefaultAsync(x => x.Id == request.Policy.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("AttendanceBreakPolicy", request.Policy.Id.Value);

            policy.Update(request.Policy, request.ModifiedBy);
        }
        else
        {
            policy = AttendanceBreakPolicy.Create(Guid.NewGuid(), request.Policy);
            await dbContext.AttendanceBreakPolicies.AddAsync(policy, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceBreakPolicyResult(policy.Adapt<AttendanceBreakPolicyDto>());
    }
}

