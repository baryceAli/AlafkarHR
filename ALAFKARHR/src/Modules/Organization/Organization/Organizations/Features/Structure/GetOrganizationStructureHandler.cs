namespace Organization.Organizations.Features.Structure;

public record GetOrganizationStructureQuery() : IQuery<GetOrganizationStructureResult>;
public record GetOrganizationStructureResult(OrganizationStructureDto Structure);

public class GetOrganizationStructureHandler(
    OrganizationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ISender sender)
    : IQueryHandler<GetOrganizationStructureQuery, GetOrganizationStructureResult>
{
    public async Task<GetOrganizationStructureResult> Handle(
        GetOrganizationStructureQuery request,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var companyIdValue = user.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var currentCompanyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var currentCompany = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentCompanyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        var canViewChildCompanies = HasPermission(user, PermissionList.CompanyPermissions.ViewChild);
        var canViewCompanyFamily = currentCompany.ParentCompanyId is null && canViewChildCompanies;
        var rootCompanyId = currentCompany.Id;

        var companyIds = canViewCompanyFamily
            ? await GetCompanyFamilyIdsAsync(rootCompanyId, cancellationToken)
            : [currentCompany.Id];

        var companies = await dbContext.Companies
            .AsNoTracking()
            .Where(x => companyIds.Contains(x.Id))
            .OrderBy(x => x.ParentCompanyId.HasValue)
            .ThenBy(x => x.Name)
            .Select(x => new OrganizationCompanyNodeDto
            {
                Id = x.Id,
                ParentCompanyId = x.ParentCompanyId,
                Name = x.Name,
                NameEng = x.NameEng,
                Code = x.Code,
                Logo = x.Logo,
                HqLocation = x.HqLocation,
                HqLongitude = x.HqLongitude,
                HqLatitude = x.HqLatitude,
                VatNo = x.VatNo,
                CurrencyId = x.CurrencyId,
                TimeZone = x.TimeZone,
                Phone = x.Phone,
                Email = x.Email,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        var companyNodes = companies.ToDictionary(x => x.Id);
        var branchNodesByCompany = await LoadBranchesAsync(companyIds, cancellationToken);
        var administrationsByScope = await LoadAdministrationsAsync(companyIds, branchNodesByCompany, cancellationToken);
        var departmentsByAdministration = await LoadDepartmentsAsync(companyIds, cancellationToken);

        foreach (var company in companies)
        {
            if (branchNodesByCompany.TryGetValue(company.Id, out var branches))
                company.Branches.AddRange(branches);

            if (administrationsByScope.CompanyAdministrations.TryGetValue(company.Id, out var administrations))
                company.Administrations.AddRange(administrations);
        }

        foreach (var administration in administrationsByScope.AllAdministrations)
        {
            if (departmentsByAdministration.TryGetValue(administration.Id, out var departments))
                administration.Departments.AddRange(departments);
        }

        foreach (var branch in branchNodesByCompany.Values.SelectMany(x => x))
        {
            branch.AdministrationCount = CountAdministrations(branch.Administrations);
            branch.DepartmentCount = branch.Administrations.Sum(CountDepartmentsForAdministration);
        }

        foreach (var company in companies)
        {
            company.BranchCount = company.Branches.Count;
            company.AdministrationCount =
                company.Administrations.Sum(CountAdministrationsForRoot)
                + company.Branches.Sum(x => x.AdministrationCount);
            company.DepartmentCount =
                company.Administrations.Sum(CountDepartmentsForAdministration)
                + company.Branches.Sum(x => x.DepartmentCount);
        }

        var roots = companies
            .Where(x => x.Id == rootCompanyId || !companyNodes.ContainsKey(x.ParentCompanyId ?? Guid.Empty))
            .OrderBy(x => x.Name)
            .ToList();

        foreach (var company in companies)
        {
            if (company.ParentCompanyId is Guid parentCompanyId
                && companyNodes.TryGetValue(parentCompanyId, out var parent))
                parent.ChildCompanies.Add(company);
        }

        var structure = new OrganizationStructureDto
        {
            Companies = roots,
            CompanyCount = companies.Count,
            BranchCount = companies.Sum(x => x.BranchCount),
            AdministrationCount = companies.Sum(x => x.AdministrationCount),
            DepartmentCount = companies.Sum(x => x.DepartmentCount)
        };

        return new GetOrganizationStructureResult(structure);
    }

    private async Task<List<Guid>> GetCompanyFamilyIdsAsync(Guid rootCompanyId, CancellationToken cancellationToken)
    {
        var companyIds = new List<Guid> { rootCompanyId };
        var pendingParentIds = new List<Guid> { rootCompanyId };

        while (pendingParentIds.Count > 0)
        {
            var childIds = await dbContext.Companies
            .AsNoTracking()
                .Where(x => x.ParentCompanyId.HasValue && pendingParentIds.Contains(x.ParentCompanyId.Value))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

            pendingParentIds = childIds.Except(companyIds).ToList();
            companyIds.AddRange(pendingParentIds);
        }

        return companyIds;
    }

    private async Task<Dictionary<Guid, List<OrganizationBranchNodeDto>>> LoadBranchesAsync(
        List<Guid> companyIds,
        CancellationToken cancellationToken)
    {
        var branchNodes = new List<OrganizationBranchNodeDto>();

        foreach (var companyId in companyIds)
        {
            var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
            var query = dbContext.Branches
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId);

            if (!access.CanViewAllBranches)
                query = query.Where(x => access.BranchIds.Contains(x.Id));

            var companyBranches = await query
                .OrderByDescending(x => x.IsMainBranch)
                .ThenBy(x => x.Name)
                .Select(x => new OrganizationBranchNodeDto
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    Name = x.Name,
                    NameEng = x.NameEng,
                    Code = x.Code,
                    Location = x.Location,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsMainBranch = x.IsMainBranch,
                    Specialization = x.Specialization
                })
                .ToListAsync(cancellationToken);

            branchNodes.AddRange(companyBranches);
        }

        return branchNodes
            .GroupBy(x => x.CompanyId)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private async Task<AdministrationScopeResult> LoadAdministrationsAsync(
        List<Guid> companyIds,
        Dictionary<Guid, List<OrganizationBranchNodeDto>> branchNodesByCompany,
        CancellationToken cancellationToken)
    {
        var visibleBranchIds = branchNodesByCompany.Values
            .SelectMany(x => x)
            .Select(x => x.Id)
            .ToHashSet();

        var administrations = await dbContext.Administrations
            .AsNoTracking()
            .Where(x => companyIds.Contains(x.CompanyId)
                && (!x.BranchId.HasValue || visibleBranchIds.Contains(x.BranchId.Value)))
            .OrderByDescending(x => x.IsHigherManagement)
            .ThenBy(x => x.Name)
            .Select(x => new OrganizationAdministrationNodeDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                BranchId = x.BranchId,
                ParentAdministrationId = x.ParentAdministrationId,
                Name = x.Name,
                NameEng = x.NameEng,
                Code = x.Code,
                ManagerId = x.ManagerId,
                IsHigherManagement = x.IsHigherManagement,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        var adminById = administrations.ToDictionary(x => x.Id);
        var companyRoots = new Dictionary<Guid, List<OrganizationAdministrationNodeDto>>();
        var branchRoots = new Dictionary<Guid, List<OrganizationAdministrationNodeDto>>();

        foreach (var administration in administrations)
        {
            if (administration.ParentAdministrationId.HasValue
                && adminById.TryGetValue(administration.ParentAdministrationId.Value, out var parent))
            {
                parent.ChildAdministrations.Add(administration);
                continue;
            }

            if (administration.BranchId.HasValue)
            {
                if (!branchRoots.TryGetValue(administration.BranchId.Value, out var list))
                {
                    list = [];
                    branchRoots[administration.BranchId.Value] = list;
                }

                list.Add(administration);
            }
            else
            {
                if (!companyRoots.TryGetValue(administration.CompanyId, out var list))
                {
                    list = [];
                    companyRoots[administration.CompanyId] = list;
                }

                list.Add(administration);
            }
        }

        var branchesById = branchNodesByCompany.Values.SelectMany(x => x).ToDictionary(x => x.Id);
        foreach (var (branchId, branchAdministrations) in branchRoots)
        {
            if (branchesById.TryGetValue(branchId, out var branch))
                branch.Administrations.AddRange(branchAdministrations);
        }

        return new AdministrationScopeResult(administrations, companyRoots);
    }

    private async Task<Dictionary<Guid, List<OrganizationDepartmentNodeDto>>> LoadDepartmentsAsync(
        List<Guid> companyIds,
        CancellationToken cancellationToken)
    {
        var departments = await dbContext.Departments
            .AsNoTracking()
            .Where(x => companyIds.Contains(x.CompanyId))
            .OrderBy(x => x.Name)
            .Select(x => new OrganizationDepartmentNodeDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                AdministrationId = x.AdministrationId,
                ParentDepartmentId = x.ParentDepartmentId,
                Name = x.Name,
                NameEng = x.NameEng,
                Code = x.Code,
                HeadOfDepartment = x.HeadOfDepartment,
                IsActive = x.IsActive,
                Location = x.Location,
                Longitude = x.Longitude,
                Latitude = x.Latitude,
                AllowedRadiusMeters = x.AllowedRadiusMeters
            })
            .ToListAsync(cancellationToken);

        var departmentById = departments.ToDictionary(x => x.Id);
        var rootsByAdministration = new Dictionary<Guid, List<OrganizationDepartmentNodeDto>>();

        foreach (var department in departments)
        {
            if (department.ParentDepartmentId.HasValue
                && departmentById.TryGetValue(department.ParentDepartmentId.Value, out var parent))
            {
                parent.ChildDepartments.Add(department);
                continue;
            }

            if (!rootsByAdministration.TryGetValue(department.AdministrationId, out var list))
            {
                list = [];
                rootsByAdministration[department.AdministrationId] = list;
            }

            list.Add(department);
        }

        return rootsByAdministration;
    }

    private static bool HasPermission(ClaimsPrincipal user, string permission)
        => user.Claims.Any(x => x.Value == permission);

    private static int CountAdministrations(List<OrganizationAdministrationNodeDto> administrations)
        => administrations.Sum(CountAdministrationsForRoot);

    private static int CountAdministrationsForRoot(OrganizationAdministrationNodeDto administration)
        => 1 + administration.ChildAdministrations.Sum(CountAdministrationsForRoot);

    private static int CountDepartmentsForAdministration(OrganizationAdministrationNodeDto administration)
        => administration.Departments.Sum(CountDepartments)
            + administration.ChildAdministrations.Sum(CountDepartmentsForAdministration);

    private static int CountDepartments(OrganizationDepartmentNodeDto department)
        => 1 + department.ChildDepartments.Sum(CountDepartments);

    private sealed record AdministrationScopeResult(
        List<OrganizationAdministrationNodeDto> AllAdministrations,
        Dictionary<Guid, List<OrganizationAdministrationNodeDto>> CompanyAdministrations);
}
