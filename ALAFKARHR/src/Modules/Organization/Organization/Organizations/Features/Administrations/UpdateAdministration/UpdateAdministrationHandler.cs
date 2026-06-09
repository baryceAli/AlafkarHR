namespace Organization.Organizations.Features.Administrations.UpdateAdministration;


public record UpdateAdministrationCommand(AdministrationDto Administration) : ICommand<UpdateAdministrationResult>;
public record UpdateAdministrationResult(bool IsSuccess);
public class UpdateAdministrationHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateAdministrationCommand, UpdateAdministrationResult>
{
    public async Task<UpdateAdministrationResult> Handle(UpdateAdministrationCommand request, CancellationToken cancellationToken)
    {
        var administration= await dbContext.Administrations.FirstOrDefaultAsync(x=> x.Id==request.Administration.Id, cancellationToken);

        if (administration is null)
            throw new NotFoundException($"Administration not found: {request.Administration.Id}");

        await ValidateParentAdministrationAsync(administration, request.Administration.ParentAdministrationId, cancellationToken);

        var userId = httpContextAccessor.HttpContext?
                        .User
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        administration.Update(
            request.Administration.Name,
            request.Administration.NameEng,
            request.Administration.IsActive,
            request.Administration.ParentAdministrationId,
            request.Administration.ManagerId,
            userId);

        await dbContext.SaveChangesAsync();
        return new UpdateAdministrationResult(true);

    }

    private async Task ValidateParentAdministrationAsync(
        Administration administration,
        Guid? parentAdministrationId,
        CancellationToken cancellationToken)
    {
        if (!parentAdministrationId.HasValue)
        {
            return;
        }

        if (parentAdministrationId.Value == administration.Id)
        {
            throw new BadRequestException("Administration cannot be a parent of itself.");
        }

        var parent = await dbContext.Administrations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == parentAdministrationId.Value, cancellationToken)
            ?? throw new NotFoundException($"Parent administration not found: {parentAdministrationId.Value}");

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

        var currentParentId = parent.ParentAdministrationId;
        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == administration.Id)
            {
                throw new BadRequestException("Administration hierarchy cannot contain a circular parent relationship.");
            }

            currentParentId = await dbContext.Administrations
                .AsNoTracking()
                .Where(x => x.Id == currentParentId.Value)
                .Select(x => x.ParentAdministrationId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
