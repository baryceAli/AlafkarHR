namespace EmployeeModule.Employees.Features.Employees;

internal static class EmployeeBranchScope
{
    public static async Task EnsureCanReadAsync(ISender sender, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(access, branchId))
            throw new ForbiddenException("You do not have permission to view this employee branch scope.");
    }

    public static async Task EnsureCanMutateAsync(ISender sender, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(access, branchId))
            throw new ForbiddenException("You do not have permission to change this employee branch scope.");
    }

    public static async Task<IQueryable<Models.Employee>> ApplyAccessAsync(ISender sender, IQueryable<Models.Employee> query, Guid companyId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        return access.CanViewAllBranches
            ? query
            : query.Where(x => access.BranchIds.Contains(x.BranchId));
    }
}
