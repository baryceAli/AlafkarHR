namespace Payroll.Salaries.Features.Components.DeleteComponent;

public class DeleteComponentHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteComponentCommand, DeleteComponentResult>
{
    public async Task<DeleteComponentResult> Handle(DeleteComponentCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var component = await dbContext.Components
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Component with ID {request.Id} not found");

        component.Remove(userId);
        dbContext.Components.Update(component);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteComponentResult(true);
    }
}
