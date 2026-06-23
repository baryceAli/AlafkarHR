namespace Maintenance.WorkOrders.Features;

internal static class MaintenanceFeatureHelpers
{
    public static Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("User is not authorized.");

        return Guid.Parse(value);
    }

    public static bool HasPermission(IHttpContextAccessor httpContextAccessor, string permission)
    {
        return httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == permission) == true;
    }

    public static IQueryable<MaintenanceWorkOrder> ApplyVisibility(
        IQueryable<MaintenanceWorkOrder> query,
        IHttpContextAccessor httpContextAccessor,
        Guid currentUserId)
    {
        if (HasPermission(httpContextAccessor, PermissionList.MaintenanceWorkOrderPermissions.ManageAll))
            return query;

        return query.Where(x =>
            x.RequestedByUserId == currentUserId ||
            x.AssignedToUserId == currentUserId.ToString() ||
            x.CreatedBy == currentUserId.ToString());
    }

    public static async Task EnsureParentAssetAsync(MaintenanceDbContext dbContext, Guid? parentAssetId, CancellationToken cancellationToken)
    {
        if (!parentAssetId.HasValue)
            return;

        var exists = await dbContext.MaintenanceAssets.AnyAsync(x => x.Id == parentAssetId.Value && !x.IsDeleted, cancellationToken);
        if (!exists)
            throw new NotFoundException("Parent maintenance asset", parentAssetId.Value);
    }

    public static async Task EnsureParentAssetScopeAsync(
        MaintenanceDbContext dbContext,
        ISender sender,
        Guid companyId,
        Guid? childBranchId,
        Guid? parentAssetId,
        Guid? currentAssetId,
        CancellationToken cancellationToken)
    {
        if (!parentAssetId.HasValue)
            return;

        if (currentAssetId.HasValue && parentAssetId.Value == currentAssetId.Value)
            throw new BadRequestException("Maintenance asset cannot be its own parent.");

        var parent = await dbContext.MaintenanceAssets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == parentAssetId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Parent maintenance asset", parentAssetId.Value);

        if (parent.CompanyId != companyId)
            throw new BadRequestException("Parent maintenance asset must belong to the same company.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, parent.BranchId))
            throw new ForbiddenException("You do not have permission to use this parent maintenance asset.");

        if (childBranchId.HasValue && parent.BranchId.HasValue && parent.BranchId != childBranchId)
            throw new BadRequestException("Branch-owned maintenance assets can only use shared parents or parents from the same branch.");

        if (!childBranchId.HasValue && parent.BranchId.HasValue)
            throw new BadRequestException("Company-level/shared maintenance assets can only use company-level/shared parents.");
    }

    public static async Task EnsureAssetAsync(MaintenanceDbContext dbContext, Guid assetId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.MaintenanceAssets.AnyAsync(x => x.Id == assetId && !x.IsDeleted, cancellationToken);
        if (!exists)
            throw new NotFoundException("Maintenance asset", assetId);
    }

    public static async Task EnsureCanMutateWorkOrderAsync(ISender sender, MaintenanceWorkOrder workOrder, CancellationToken cancellationToken)
    {
        if (workOrder.Asset is null)
            throw new NotFoundException("Maintenance asset", workOrder.AssetId);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(workOrder.Asset.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, workOrder.Asset.BranchId))
            throw new ForbiddenException("You do not have permission to change this maintenance work order branch scope.");
    }

    public static MaintenanceWorkOrderDto ToDto(MaintenanceWorkOrder workOrder)
    {
        return new MaintenanceWorkOrderDto
        {
            Id = workOrder.Id,
            WorkOrderNumber = workOrder.WorkOrderNumber,
            Title = workOrder.Title,
            Description = workOrder.Description,
            AssetId = workOrder.AssetId,
            AssetName = workOrder.Asset?.Name ?? string.Empty,
            AssetType = workOrder.Asset?.AssetType ?? MaintenanceAssetType.Other,
            BranchId = workOrder.Asset?.BranchId,
            RequestedByUserId = workOrder.RequestedByUserId,
            AssignedToUserId = workOrder.AssignedToUserId,
            Priority = workOrder.Priority,
            Status = workOrder.Status,
            RequestedDate = workOrder.RequestedDate,
            DueDate = workOrder.DueDate,
            StartedAt = workOrder.StartedAt,
            CompletedAt = workOrder.CompletedAt,
            Category = workOrder.Category,
            InternalNotes = workOrder.InternalNotes,
            EstimatedCost = workOrder.EstimatedCost,
            ApprovedCost = workOrder.ApprovedCost,
            ActualCost = workOrder.ActualCost,
            CurrencyId = workOrder.CurrencyId,
            CurrencyCode = workOrder.CurrencyCode,
            VendorName = workOrder.VendorName,
            SupplierId = workOrder.SupplierId,
            CostApprovalStatus = workOrder.CostApprovalStatus,
            ApprovedByUserId = workOrder.ApprovedByUserId,
            ApprovedAt = workOrder.ApprovedAt,
            ApprovalNotes = workOrder.ApprovalNotes,
            Comments = workOrder.Comments.Select(x => x.Adapt<MaintenanceCommentDto>()).ToList(),
            Attachments = workOrder.Attachments.Select(x => x.Adapt<MaintenanceAttachmentDto>()).ToList(),
            History = workOrder.History.Select(x => x.Adapt<MaintenanceHistoryDto>()).ToList()
        };
    }

    public static void AddHistory(MaintenanceWorkOrder workOrder, string action, string? details, Guid userId)
    {
        workOrder.AddHistory(MaintenanceHistory.Create(workOrder.Id, action, details, userId));
    }
}
