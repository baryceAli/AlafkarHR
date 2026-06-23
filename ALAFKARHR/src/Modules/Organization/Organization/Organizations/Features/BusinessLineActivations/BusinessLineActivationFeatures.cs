using SharedWithUI.Organization;
using SharedWithUI.Organization.Enums;

namespace Organization.Organizations.Features.BusinessLineActivations;

public record GetBusinessLineActivationsQuery(Guid? CompanyId, Guid? BusinessLineId) : IQuery<GetBusinessLineActivationsResult>;
public record GetBusinessLineActivationsResult(List<BusinessLineActivationDto> Activations);
public record SetBusinessLineActivationCommand(Guid CompanyId, Guid BusinessLineId, bool IsActive) : ICommand<SetBusinessLineActivationResult>;
public record SetBusinessLineActivationResult(bool IsSuccess);
public record SetBusinessLineActivationRequest(Guid CompanyId, Guid BusinessLineId, bool IsActive);
public record GetBusinessLineActivationsResponse(List<BusinessLineActivationDto> Activations);

public class BusinessLineActivationValidator : AbstractValidator<SetBusinessLineActivationCommand>
{
    public BusinessLineActivationValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BusinessLineId).NotEmpty();
    }
}

public class BusinessLineActivationQueryHandler(OrganizationDbContext dbContext, ICompanyHierarchyContext companyHierarchyContext)
    : IQueryHandler<GetBusinessLineActivationsQuery, GetBusinessLineActivationsResult>
{
    public async Task<GetBusinessLineActivationsResult> Handle(GetBusinessLineActivationsQuery request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetCurrentParentCompanyIdAsync(cancellationToken);
        var hierarchyIds = await companyHierarchyContext.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);

        var query = dbContext.BusinessLineActivations
            .AsNoTracking()
            .Include(x => x.BusinessLine)
            .Where(x => x.ParentCompanyId == parentCompanyId && hierarchyIds.Contains(x.CompanyId));

        if (request.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.CompanyId.Value);
        if (request.BusinessLineId.HasValue)
            query = query.Where(x => x.BusinessLineId == request.BusinessLineId.Value);

        var activations = await query
            .OrderBy(x => x.BusinessLine.DisplayOrder)
            .ThenBy(x => x.CompanyId)
            .Select(x => new BusinessLineActivationDto
            {
                Id = x.Id,
                ParentCompanyId = x.ParentCompanyId,
                CompanyId = x.CompanyId,
                BusinessLineId = x.BusinessLineId,
                BusinessLineKey = x.BusinessLine.Key,
                BusinessLineName = x.BusinessLine.Name,
                BusinessLineNameAr = x.BusinessLine.NameAr,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new GetBusinessLineActivationsResult(activations);
    }
}

public class BusinessLineActivationCommandHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICompanyHierarchyContext companyHierarchyContext)
    : ICommandHandler<SetBusinessLineActivationCommand, SetBusinessLineActivationResult>
{
    public async Task<SetBusinessLineActivationResult> Handle(SetBusinessLineActivationCommand request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetCurrentParentCompanyIdAsync(cancellationToken);
        var hierarchyIds = await companyHierarchyContext.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);
        if (!hierarchyIds.Contains(request.CompanyId))
            throw new InvalidOperationException("Company is outside the licensed company hierarchy");

        var businessLine = await dbContext.BusinessLines.FirstOrDefaultAsync(x => x.Id == request.BusinessLineId, cancellationToken)
            ?? throw new NotFoundException($"Business line not found: {request.BusinessLineId}");

        if (businessLine.ActivationPolicy != BusinessLineActivationPolicy.SinglePerCompany)
            throw new InvalidOperationException("Only single-per-company business lines use explicit activations");

        if (businessLine.Key == BusinessLineKeys.StoreFront)
            throw new InvalidOperationException("Store Front activations are consumed by active stores");

        var license = await dbContext.CompanyLicenses.FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken)
            ?? throw new InvalidOperationException("Parent company does not have a license");
        if (!license.AllowsAccess(DateTime.UtcNow))
            throw new InvalidOperationException("Parent company license is not active");

        var licensedLine = await dbContext.CompanyLicenseBusinessLines
            .FirstOrDefaultAsync(x => x.CompanyLicenseId == license.Id && x.BusinessLineId == request.BusinessLineId, cancellationToken)
            ?? throw new InvalidOperationException("Business line is not licensed");

        var activation = await dbContext.BusinessLineActivations
            .FirstOrDefaultAsync(x => x.ParentCompanyId == parentCompanyId &&
                                      x.CompanyId == request.CompanyId &&
                                      x.BusinessLineId == request.BusinessLineId,
                cancellationToken);

        if (request.IsActive)
        {
            var activeCount = await dbContext.BusinessLineActivations
                .CountAsync(x => x.ParentCompanyId == parentCompanyId &&
                                 x.BusinessLineId == request.BusinessLineId &&
                                 x.IsActive &&
                                 (activation == null || x.Id != activation.Id),
                    cancellationToken);
            if (activeCount >= licensedLine.ActivationLimit)
                throw new InvalidOperationException("Business line activation limit has been reached");
        }

        var userId = GetUserId();
        if (activation is null)
        {
            if (request.IsActive)
                await dbContext.BusinessLineActivations.AddAsync(BusinessLineActivation.Create(parentCompanyId, request.CompanyId, request.BusinessLineId, userId), cancellationToken);
        }
        else
        {
            activation.SetActive(request.IsActive, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new SetBusinessLineActivationResult(true);
    }

    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}

public class BusinessLineActivationEndpoints : ICarterModule
{
    private const string Route = "/api/v1/organization/business-line-activations";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(Route, async (Guid? companyId, Guid? businessLineId, ISender sender) =>
        {
            var result = await sender.Send(new GetBusinessLineActivationsQuery(companyId, businessLineId));
            return Results.Ok(result.Adapt<GetBusinessLineActivationsResponse>());
        })
        .WithName("GetBusinessLineActivations")
        .Produces<GetBusinessLineActivationsResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.CompanyPermissions.ViewLicense);

        app.MapPost(Route, async (SetBusinessLineActivationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SetBusinessLineActivationCommand(request.CompanyId, request.BusinessLineId, request.IsActive));
            return Results.Ok(result);
        })
        .WithName("SetBusinessLineActivation")
        .Produces<SetBusinessLineActivationResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.CompanyPermissions.Edit);
    }
}
