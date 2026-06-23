namespace Shared.Contracts.Organization;

public static class BranchScopePolicy
{
    public static bool CanRead(GetCurrentUserBranchAccessResult access, Guid? branchId)
    {
        return access.CanViewAllBranches
            || !branchId.HasValue
            || access.BranchIds.Contains(branchId.Value);
    }

    public static bool CanMutate(GetCurrentUserBranchAccessResult access, Guid? branchId)
    {
        return access.CanViewAllBranches
            || (branchId.HasValue && access.BranchIds.Contains(branchId.Value));
    }

    public static bool CanFilter(GetCurrentUserBranchAccessResult access, Guid? branchId)
    {
        return access.CanViewAllBranches
            || !branchId.HasValue
            || access.BranchIds.Contains(branchId.Value);
    }
}
