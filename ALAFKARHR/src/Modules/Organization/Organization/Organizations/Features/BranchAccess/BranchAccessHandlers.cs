using Auth.Contracts.Features.UserCompanyMembership;
using Shared.Contracts.Organization;

namespace Organization.Organizations.Features.BranchAccess;

public record GetBranchRoleProfilesQuery() : IQuery<GetBranchRoleProfilesResult>;
public record GetBranchRoleProfilesResult(List<BranchRoleProfileDto> Profiles);
public record AssignUserBranchRoleCommand(Guid UserId, Guid CompanyId, Guid BranchId, string TemplateKey) : ICommand<AssignUserBranchRoleResult>;
public record AssignUserBranchRoleResult(Guid Id);
public record RemoveUserBranchRoleCommand(Guid AssignmentId) : ICommand<RemoveUserBranchRoleResult>;
public record RemoveUserBranchRoleResult(bool IsSuccess);
public record GetUserBranchRoleAssignmentsQuery(Guid UserId, Guid CompanyId) : IQuery<GetUserBranchRoleAssignmentsResult>;
public record GetUserBranchRoleAssignmentsResult(List<BranchRoleAssignmentDto> Assignments);
public record GetCompanyBranchRoleAssignmentsQuery(Guid CompanyId, Guid? BranchId = null) : IQuery<GetCompanyBranchRoleAssignmentsResult>;
public record GetCompanyBranchRoleAssignmentsResult(List<BranchRoleAssignmentDto> Assignments);
public record GetCurrentUserBranchRoleAccessQuery(Guid CompanyId) : IQuery<GetCurrentUserBranchRoleAccessResult>;
public record GetCurrentUserBranchRoleAccessResult(CurrentUserBranchRoleAccessDto Access);

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
      ICommandHandler<EnsureUserBranchAccessCommand, EnsureUserBranchAccessResult>,
      ICommandHandler<AssignUserBranchRoleCommand, AssignUserBranchRoleResult>,
      ICommandHandler<AssignStoreFrontBranchRoleCommand, AssignStoreFrontBranchRoleResult>,
      ICommandHandler<RevokeStoreFrontBranchRoleCommand, RevokeStoreFrontBranchRoleResult>,
      ICommandHandler<RemoveUserBranchRoleCommand, RemoveUserBranchRoleResult>,
      ICommandHandler<EnsureStoreFrontBranchCommand, EnsureStoreFrontBranchResult>,
      IQueryHandler<GetCurrentUserBranchAccessQuery, GetCurrentUserBranchAccessResult>,
      IQueryHandler<GetCompanyBranchesForAccountingQuery, GetCompanyBranchesForAccountingResult>,
      IQueryHandler<GetUserBranchAssignmentsQuery, GetUserBranchAssignmentsResult>,
      IQueryHandler<GetCompanyUserBranchAssignmentsQuery, GetCompanyUserBranchAssignmentsResult>,
      IQueryHandler<GetBranchScopeInfoQuery, GetBranchScopeInfoResult>,
      IQueryHandler<EnsureCurrentUserBranchPermissionQuery, EnsureCurrentUserBranchPermissionResult>,
      IQueryHandler<GetCurrentUserBranchRolePermissionsQuery, GetCurrentUserBranchRolePermissionsResult>,
      IQueryHandler<GetCurrentUserBranchRoleAccessForAuthorizationQuery, GetCurrentUserBranchRoleAccessForAuthorizationResult>,
      IQueryHandler<GetBranchRoleProfilesQuery, GetBranchRoleProfilesResult>,
      IQueryHandler<GetUserBranchRoleAssignmentsQuery, GetUserBranchRoleAssignmentsResult>,
      IQueryHandler<GetCompanyBranchRoleAssignmentsForDashboardQuery, GetCompanyBranchRoleAssignmentsForDashboardResult>,
      IQueryHandler<GetCompanyBranchRoleAssignmentsQuery, GetCompanyBranchRoleAssignmentsResult>,
      IQueryHandler<GetCurrentUserBranchRoleAccessQuery, GetCurrentUserBranchRoleAccessResult>
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
                legacyMainCodeBranch.Specialization,
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
            BranchSpecialization.General,
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

    public async Task<EnsureUserBranchAccessResult> Handle(EnsureUserBranchAccessCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
            throw new BadRequestException("User is required.");
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (request.BranchId == Guid.Empty)
            throw new BadRequestException("Branch is required.");

        var currentUserId = CurrentUserId();
        var userMembership = await sender.Send(new UserBelongsToCompanyQuery(request.UserId, request.CompanyId), cancellationToken);
        if (!userMembership.BelongsToCompany)
            throw new BadRequestException("User does not belong to the selected company.");

        var branchExists = await dbContext.Branches.AsNoTracking()
            .AnyAsync(x => x.Id == request.BranchId && x.CompanyId == request.CompanyId, cancellationToken);
        if (!branchExists)
            throw new BadRequestException("Branch does not belong to the selected company.");

        var assignments = await dbContext.UserBranchAssignments
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var assignment = assignments.FirstOrDefault(x => x.BranchId == request.BranchId);
        if (assignment is null)
        {
            await dbContext.UserBranchAssignments.AddAsync(
                UserBranchAssignment.Create(request.UserId, request.CompanyId, request.BranchId, request.MakeDefault, currentUserId),
                cancellationToken);
        }
        else if (request.MakeDefault && !assignment.IsDefault)
        {
            assignment.SetDefault(true, currentUserId);
        }

        if (request.MakeDefault)
        {
            foreach (var otherAssignment in assignments.Where(x => x.BranchId != request.BranchId && x.IsDefault))
                otherAssignment.SetDefault(false, currentUserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureUserBranchAccessResult(true);
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

    public async Task<GetCompanyUserBranchAssignmentsResult> Handle(GetCompanyUserBranchAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await dbContext.UserBranchAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Select(x => new UserBranchAssignmentInfo(x.UserId, x.CompanyId, x.BranchId, x.IsDefault))
            .ToListAsync(cancellationToken);

        return new GetCompanyUserBranchAssignmentsResult(assignments);
    }

    public Task<GetBranchRoleProfilesResult> Handle(GetBranchRoleProfilesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new GetBranchRoleProfilesResult(BranchRoleProfiles.All.Select(CloneProfile).ToList()));

    public async Task<EnsureStoreFrontBranchResult> Handle(EnsureStoreFrontBranchCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        var userId = string.IsNullOrWhiteSpace(request.UserId) ? CurrentUserId() : request.UserId;

        if (request.BranchId.HasValue && request.BranchId.Value != Guid.Empty)
        {
            var branch = await dbContext.Branches.FirstOrDefaultAsync(x => x.Id == request.BranchId.Value, cancellationToken)
                ?? throw new NotFoundException($"Branch not found: {request.BranchId.Value}");
            if (branch.CompanyId != request.CompanyId)
                throw new BadRequestException("Selected branch does not belong to the selected company.");
            if (branch.Specialization != BranchSpecialization.StoreFront)
                throw new BadRequestException("Selected branch must be a StoreFront branch.");
            return new EnsureStoreFrontBranchResult(branch.Id);
        }

        var code = NormalizeCode(request.Code);
        var existing = await dbContext.Branches
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.Code == code, cancellationToken);
        if (existing is not null)
        {
            if (existing.Specialization != BranchSpecialization.StoreFront)
                throw new BadRequestException("A non-StoreFront branch already uses this store code.");
            return new EnsureStoreFrontBranchResult(existing.Id);
        }

        var company = await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException($"Company not found: {request.CompanyId}");

        var createdBranch = Branch.Create(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(request.Name) ? request.NameEng : request.Name,
            string.IsNullOrWhiteSpace(request.NameEng) ? request.Name : request.NameEng,
            company.HqLocation,
            company.HqLongitude,
            company.HqLatitude,
            code,
            string.IsNullOrWhiteSpace(request.Phone) ? company.Phone : request.Phone,
            string.IsNullOrWhiteSpace(request.Email) ? company.Email : request.Email,
            false,
            BranchSpecialization.StoreFront,
            request.CompanyId,
            userId);

        await dbContext.Branches.AddAsync(createdBranch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await sender.Send(new EnsureBranchAccountingCommand(
            createdBranch.CompanyId,
            createdBranch.Id,
            createdBranch.Code,
            createdBranch.Name,
            createdBranch.NameEng), cancellationToken);

        return new EnsureStoreFrontBranchResult(createdBranch.Id);
    }

    public async Task<GetBranchScopeInfoResult> Handle(GetBranchScopeInfoQuery request, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.BranchId && x.CompanyId == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException($"Branch not found: {request.BranchId}");
        return new GetBranchScopeInfoResult(branch.Id, branch.CompanyId, (int)branch.Specialization);
    }

    public async Task<AssignUserBranchRoleResult> Handle(AssignUserBranchRoleCommand request, CancellationToken cancellationToken)
    {
        var assignmentId = await AssignStoreFrontBranchRoleAsync(request.UserId, request.CompanyId, request.BranchId, request.TemplateKey, cancellationToken);
        return new AssignUserBranchRoleResult(assignmentId);
    }

    public async Task<AssignStoreFrontBranchRoleResult> Handle(AssignStoreFrontBranchRoleCommand request, CancellationToken cancellationToken)
    {
        var assignmentId = await AssignStoreFrontBranchRoleAsync(request.UserId, request.CompanyId, request.BranchId, request.TemplateKey, cancellationToken);
        return new AssignStoreFrontBranchRoleResult(assignmentId);
    }

    public async Task<RevokeStoreFrontBranchRoleResult> Handle(RevokeStoreFrontBranchRoleCommand request, CancellationToken cancellationToken)
    {
        var profile = BranchRoleProfiles.GetRequired(request.TemplateKey);
        var assignment = await dbContext.UserBranchRoleAssignments
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId
                && x.UserId == request.UserId
                && x.BranchId == request.BranchId
                && x.TemplateKey == profile.TemplateKey, cancellationToken);

        if (assignment is null)
            return new RevokeStoreFrontBranchRoleResult(true);

        assignment.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RevokeStoreFrontBranchRoleResult(true);
    }

    public async Task<RemoveUserBranchRoleResult> Handle(RemoveUserBranchRoleCommand request, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.UserBranchRoleAssignments.FirstOrDefaultAsync(x => x.Id == request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException($"Branch role assignment not found: {request.AssignmentId}");
        assignment.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveUserBranchRoleResult(true);
    }

    public async Task<GetUserBranchRoleAssignmentsResult> Handle(GetUserBranchRoleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == request.UserId)
            .OrderBy(x => x.BranchId)
            .ThenBy(x => x.TemplateKey)
            .ToListAsync(cancellationToken);

        return new GetUserBranchRoleAssignmentsResult(assignments.Select(ToDto).ToList());
    }

    public async Task<GetCompanyBranchRoleAssignmentsResult> Handle(GetCompanyBranchRoleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (request.BranchId.HasValue && request.BranchId.Value != Guid.Empty)
            query = query.Where(x => x.BranchId == request.BranchId.Value);

        var assignments = await query
            .OrderBy(x => x.BranchId)
            .ThenBy(x => x.UserId)
            .ThenBy(x => x.TemplateKey)
            .ToListAsync(cancellationToken);

        return new GetCompanyBranchRoleAssignmentsResult(assignments.Select(ToDto).ToList());
    }

    public async Task<GetCompanyBranchRoleAssignmentsForDashboardResult> Handle(GetCompanyBranchRoleAssignmentsForDashboardQuery request, CancellationToken cancellationToken)
    {
        var assignments = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Select(x => new BranchRoleAssignmentInfo(x.Id, x.UserId, x.CompanyId, x.BranchId, x.TemplateKey, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new GetCompanyBranchRoleAssignmentsForDashboardResult(assignments);
    }

    public async Task<GetCurrentUserBranchRoleAccessResult> Handle(GetCurrentUserBranchRoleAccessQuery request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserGuid();
        var assignments = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == userId)
            .ToListAsync(cancellationToken);
        var dtoAssignments = assignments.Select(ToDto).ToList();
        return new GetCurrentUserBranchRoleAccessResult(new CurrentUserBranchRoleAccessDto
        {
            CompanyId = request.CompanyId,
            Assignments = dtoAssignments,
            EffectivePermissions = dtoAssignments.SelectMany(x => x.Permissions).Distinct(StringComparer.Ordinal).ToList()
        });
    }

    public async Task<GetCurrentUserBranchRolePermissionsResult> Handle(GetCurrentUserBranchRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId ?? CurrentUserGuid();
        var templateKeys = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == userId)
            .Select(x => x.TemplateKey)
            .Distinct()
            .ToListAsync(cancellationToken);
        var permissions = templateKeys
            .Select(BranchRoleProfiles.GetRequired)
            .SelectMany(x => x.Permissions)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        return new GetCurrentUserBranchRolePermissionsResult(permissions);
    }

    public async Task<GetCurrentUserBranchRoleAccessForAuthorizationResult> Handle(GetCurrentUserBranchRoleAccessForAuthorizationQuery request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserGuid();
        var assignments = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == userId)
            .Select(x => new { x.BranchId, x.TemplateKey })
            .ToListAsync(cancellationToken);

        var access = assignments
            .GroupBy(x => x.BranchId)
            .Select(group => new BranchRolePermissionAccess(
                group.Key,
                group.SelectMany(x => BranchRoleProfiles.GetRequired(x.TemplateKey).Permissions).Distinct(StringComparer.Ordinal).ToList()))
            .ToList();
        return new GetCurrentUserBranchRoleAccessForAuthorizationResult(access);
    }

    public async Task<EnsureCurrentUserBranchPermissionResult> Handle(EnsureCurrentUserBranchPermissionQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException("User is not authenticated");
        if (user.Claims.Any(x => x.Type == "Permission" && x.Value == request.Permission))
            return new EnsureCurrentUserBranchPermissionResult(true);

        var userId = CurrentUserGuid();
        var templateKeys = await dbContext.UserBranchRoleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.UserId == userId && x.BranchId == request.BranchId)
            .Select(x => x.TemplateKey)
            .ToListAsync(cancellationToken);
        var hasScopedPermission = templateKeys.Any(templateKey => BranchRoleProfiles.HasPermission(templateKey, request.Permission));
        if (!hasScopedPermission)
            throw new ForbiddenException("You do not have permission for this store branch.");

        return new EnsureCurrentUserBranchPermissionResult(true);
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

    private static BranchRoleAssignmentDto ToDto(UserBranchRoleAssignment assignment)
    {
        var profile = BranchRoleProfiles.GetRequired(assignment.TemplateKey);
        return new BranchRoleAssignmentDto
        {
            Id = assignment.Id,
            UserId = assignment.UserId,
            CompanyId = assignment.CompanyId,
            BranchId = assignment.BranchId,
            TemplateKey = profile.TemplateKey,
            RoleName = profile.Name,
            RoleNameAr = profile.NameAr,
            Permissions = profile.Permissions.ToList()
        };
    }

    private static BranchRoleProfileDto CloneProfile(BranchRoleProfileDto profile) => new()
    {
        TemplateKey = profile.TemplateKey,
        Name = profile.Name,
        NameAr = profile.NameAr,
        Permissions = profile.Permissions.ToList()
    };

    private async Task<Guid> AssignStoreFrontBranchRoleAsync(Guid userId, Guid companyId, Guid branchId, string templateKey, CancellationToken cancellationToken)
    {
        var profile = BranchRoleProfiles.GetRequired(templateKey);
        var currentUserId = CurrentUserId();

        var userMembership = await sender.Send(new UserBelongsToCompanyQuery(userId, companyId), cancellationToken);
        if (!userMembership.BelongsToCompany)
            throw new BadRequestException("User does not belong to the selected company.");

        var branch = await dbContext.Branches.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == branchId && x.CompanyId == companyId, cancellationToken)
            ?? throw new BadRequestException("Branch does not belong to the selected company.");
        if (branch.Specialization != BranchSpecialization.StoreFront)
            throw new BadRequestException("Store roles can only be assigned to StoreFront branches.");

        var existing = await dbContext.UserBranchRoleAssignments
            .FirstOrDefaultAsync(x => x.CompanyId == companyId
                && x.UserId == userId
                && x.BranchId == branchId
                && x.TemplateKey == profile.TemplateKey, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var assignment = UserBranchRoleAssignment.Create(userId, companyId, branchId, profile.TemplateKey, currentUserId);
        await dbContext.UserBranchRoleAssignments.AddAsync(assignment, cancellationToken);

        var hasBranchAccess = await dbContext.UserBranchAssignments
            .AnyAsync(x => x.CompanyId == companyId && x.UserId == userId && x.BranchId == branchId, cancellationToken);
        if (!hasBranchAccess)
        {
            await dbContext.UserBranchAssignments.AddAsync(
                UserBranchAssignment.Create(userId, companyId, branchId, false, currentUserId),
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return assignment.Id;
    }

    private static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
}
