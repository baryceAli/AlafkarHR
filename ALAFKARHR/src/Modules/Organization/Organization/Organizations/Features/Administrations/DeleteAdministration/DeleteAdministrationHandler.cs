namespace Organization.Organizations.Features.Administrations.DeleteAdministration;

public record DeleteAdministrationCommand(Guid Id) : ICommand<DeleteAdministrationResult>;
public record DeleteAdministrationResult(bool IsSuccess);
public class DeleteAdministrationHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteAdministrationCommand, DeleteAdministrationResult>
{
    public async Task<DeleteAdministrationResult> Handle(DeleteAdministrationCommand request, CancellationToken cancellationToken)
    {
        var administration = await dbContext.Administrations.FirstOrDefaultAsync(x=> x.Id == request.Id, cancellationToken);

        if (administration is null)
            throw new NotFoundException($"Administration not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        administration.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new DeleteAdministrationResult(true);
    }
}
