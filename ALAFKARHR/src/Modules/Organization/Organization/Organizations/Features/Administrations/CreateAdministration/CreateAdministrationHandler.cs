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

        var branch = await dbContext.Branches.FindAsync([request.Administration.BranchId]);
        if (branch is null)
            throw new NotFoundException($"Branch not found: {request.Administration.BranchId}");

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
            request.Administration.BranchId.Value,
            request.Administration.ManagerId,
            request.Administration.IsActive,
            request.Administration.CompanyId,
            userId);

        await dbContext.Administrations.AddAsync(administration, cancellationToken);
        await dbContext.SaveChangesAsync();

        return new CreateAdministrationResult(administration.Adapt<AdministrationDto>());
    }
}
