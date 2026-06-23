using Auth.Contracts.Features.UserCompanyMembership;
using Shared.Contracts.Organization;

namespace Organization.Organizations.Features.BranchAccess;

public record AssignUserBranchesCommand(Guid UserId, Guid CompanyId, List<Guid> BranchIds, Guid? DefaultBranchId) : ICommand<AssignUserBranchesResult>;

public record AssignUserBranchesResult(int AssignedCount);

public class BranchAccessCommandValidator : AbstractValidator<AssignUserBranchesCommand>
{
    public BranchAccessCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchIds).NotNull();
    }
}

public class BranchAccessHandlers(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<EnsureMainBranchCommand, EnsureMainBranchResult>,
      ICommandHandler<AssignUserBranchesCommand, AssignUserBranchesResult>,
      IQueryHandler<GetCurrentUserBranchAccessQuery, GetCurrentUserBranchAccessResult>,
      IQueryHandler<GetCompanyBranchesForAccountingQuery, GetCompanyBranchesForAccountingResult>,
      IQueryHandler<GetUserBranchAssignmentsQuery, GetUserBranchAssignmentsResult>
{
    public async Task<EnsureMainBranchResult> Handle(EnsureMainBranchCommand request, CancellationToken cancellationToken)
    {
        var existingMain = await dbContext.Branches
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.IsMainBranch, cancellationToken);
        if (existingMain is not null)
            return new EnsureMainBranchResult(existingMain.Id);

        var legacyMainCodeBranch = await dbContext.Branches
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.Code == "MAIN", cancellationToken);
        if (legacyMainCodeBranch is not null)
        {
            legacyMainCodeBranch.Update(
                legacyMainCodeBranch.Name,
                legacyMainCodeBranch.NameEng,
                legacyMainCodeBranch.Location,
                legacyMainCodeBranch.Longitude,
                legacyMainCodeBranch.Latitude,
                legacyMainCodeBranch.Code,
                legacyMainCodeBranch.Phone,
                legacyMainCodeBranch.Email,
                true,
                request.UserId);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new EnsureMainBranchResult(legacyMainCodeBranch.Id);
        }

        var company = await dbContext.Companies
            .FirstOrDefaultAsync(x => x.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException($"Company not found: {request.CompanyId}");

        var branch = Branch.Create(
            Guid.NewGuid(),
            company.Name,
            company.NameEng,
            company.HqLocation,
            company.HqLongitude,
            company.HqLatitude,
            "MAIN",
            company.Phone,
            company.Email,
            true,
            company.Id,
            request.UserId);

        await dbContext.Branches.AddAsync(branch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureMainBranchResult(branch.Id);
    }

    public async Task<AssignUserBranchesResult> Handle(AssignUserBranchesCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId();
        var branchIds = request.BranchIds.Where(x => x != Guid.Empty).Distinct().ToList();
        if (request.DefaultBranchId.HasValue && !branchIds.Contains(request.DefaultBranchId.Value))
            throw new BadRequestException("Default branch must be included in assigned branches.");

        var userMembership = await sender.Send(new UserBelongsToCompanyQuery(request.UserId, request.CompanyId), cancellationToken);
        if (!userMembership.BelongsToCompany)
            throw new BadRequestException("User does not belong to the selected company.");

        var validBranchIds = await dbContext.Branches.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && branchIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        if (validBranchIds.Count != branchIds.Count)
            throw new BadRequestException("One or more branches do not belong to the selected company.");

        var existing = await dbContext.UserBranchAssignments
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        foreach (var assignment in existing.Where(x => !branchIds.Contains(x.BranchId)))
            assignment.Remove(userId);

        var existingBranchIds = existing.Where(x => !x.IsDeleted).Select(x => x.BranchId).ToHashSet();
        foreach (var branchId in branchIds.Where(x => !existingBranchIds.Contains(x)))
        {
            await dbContext.UserBranchAssignments.AddAsync(
                UserBranchAssignment.Create(request.UserId, request.CompanyId, branchId, request.DefaultBranchId == branchId, userId),
                cancellationToken);
        }

        foreach (var assignment in existing.Where(x => branchIds.Contains(x.BranchId)))
            assignment.SetDefault(request.DefaultBranchId == assignment.BranchId, userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AssignUserBranchesResult(branchIds.Count);
    }

    public async Task<GetCurrentUserBranchAccessResult> Handle(GetCurrentUserBranchAccessQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException("User is not authenticated");
        var canViewAll =
            user.HasClaim(x => x.Value == PermissionList.OrganizationBranchAccessPermissions.ViewAll) ||
            user.HasClaim(x => x.Value == PermissionList.AccountingBranchAccessPermissions.ViewAll);
        if (canViewAll)
            return new GetCurrentUserBranchAccessResult(true, []);

        var userId = CurrentUserGuid();
        var branchIds = await dbContext.UserBranchAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == userId)
            .Select(x => x.BranchId)
            .ToListAsync(cancellationToken);

        return new GetCurrentUserBranchAccessResult(false, branchIds);
    }

    public async Task<GetCompanyBranchesForAccountingResult> Handle(GetCompanyBranchesForAccountingQuery request, CancellationToken cancellationToken)
    {
        var branches = await dbContext.Branches.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.IsMainBranch)
            .ThenBy(x => x.NameEng)
            .Select(x => new BranchAccountingInfo(x.Id, x.CompanyId, x.Code, x.Name, x.NameEng, x.IsMainBranch))
            .ToListAsync(cancellationToken);
        return new GetCompanyBranchesForAccountingResult(branches);
    }

    public async Task<GetUserBranchAssignmentsResult> Handle(GetUserBranchAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await dbContext.UserBranchAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == request.UserId)
            .Select(x => new { x.BranchId, x.IsDefault })
            .ToListAsync(cancellationToken);

        return new GetUserBranchAssignmentsResult(
            assignments.Select(x => x.BranchId).ToList(),
            assignments.FirstOrDefault(x => x.IsDefault)?.BranchId);
    }

    private string CurrentUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
        ?? "system";

    private Guid CurrentUserGuid()
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User identifier is not valid.");
    }
}
