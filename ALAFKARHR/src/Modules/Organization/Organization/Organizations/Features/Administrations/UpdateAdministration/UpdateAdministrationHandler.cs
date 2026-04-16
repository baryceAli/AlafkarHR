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

        var userId = httpContextAccessor.HttpContext?
                        .User
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        administration.Update(
            request.Administration.Name,
            request.Administration.NameEng,
            request.Administration.IsActive,
            request.Administration.ManagerId,
            userId);

        await dbContext.SaveChangesAsync();
        return new UpdateAdministrationResult(true);

    }
}
