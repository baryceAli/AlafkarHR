namespace Organization.Organizations.Features.Administrations.CreateAdministration;


public record CreateAdministrationCommand(AdministrationDto Administration) : ICommand<CreateAdministrationResult>;
public record CreateAdministrationResult(AdministrationDto CreatedAdministration);
public class CreateAdministrationHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateAdministrationCommand, CreateAdministrationResult>
{
    public async Task<CreateAdministrationResult> Handle(CreateAdministrationCommand request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.FindAsync([request.Administration.CompanyId]);
        if (company is null)
            throw new NotFoundException($"Company not found: {request.Administration.CompanyId}");

        Guid? branchId = null;
        if (request.Administration.BranchId.HasValue)
        {
            var branchIdValue = request.Administration.BranchId.Value;
            var branch = await dbContext.Branches.FindAsync([branchIdValue]);
            if (branch is null)
                throw new NotFoundException($"Branch not found: {branchIdValue}");

            branchId = branchIdValue;
        }

        await ValidateParentAdministrationAsync(request.Administration, cancellationToken);

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var administration = Administration.Create(
            Guid.NewGuid(),
            request.Administration.Name,
            request.Administration.NameEng,
            request.Administration.Code,
            branchId,
            request.Administration.ParentAdministrationId,
            request.Administration.ManagerId,
            request.Administration.IsHigherManagement,
            request.Administration.IsActive,
            request.Administration.CompanyId,
            userId);

        await dbContext.Administrations.AddAsync(administration, cancellationToken);
        await dbContext.SaveChangesAsync();

        return new CreateAdministrationResult(administration.Adapt<AdministrationDto>());
    }

    private async Task ValidateParentAdministrationAsync(
        AdministrationDto administration,
        CancellationToken cancellationToken)
    {
        if (!administration.ParentAdministrationId.HasValue)
        {
            return;
        }

        var parent = await dbContext.Administrations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == administration.ParentAdministrationId.Value, cancellationToken)
            ?? throw new NotFoundException($"Parent administration not found: {administration.ParentAdministrationId.Value}");

        if (parent.CompanyId != administration.CompanyId)
        {
            throw new BadRequestException("Parent administration must belong to the same company.");
        }

        if (!administration.BranchId.HasValue && parent.BranchId.HasValue)
        {
            throw new BadRequestException("Company-level administration cannot be assigned under a branch administration.");
        }

        if (administration.BranchId.HasValue
            && parent.BranchId.HasValue
            && parent.BranchId != administration.BranchId)
        {
            throw new BadRequestException("Branch administration cannot be assigned under a parent from another branch.");
        }
    }
}
