namespace SalesOrder.Orders.Features;

internal static class SalesOrderBranchScope
{
    public static async Task EnsureCanMutateAsync(ISender sender, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(access, branchId))
            throw new ForbiddenException("You do not have permission to change this sales branch scope.");
    }
}
