namespace Payroll.Salaries.Features.Components.UpdateComponent;

public class UpdateComponentHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateComponentCommand, UpdateComponentResult>
{
    public async Task<UpdateComponentResult> Handle(UpdateComponentCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var component = await dbContext.Components
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.CompanyId == request.Component.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Component with ID {request.Id} not found");

        component.Update(
            request.Component.Name,
            request.Component.NameEng,
            request.Component.ComponentType,
            request.Component.IsTaxable,
            request.Component.Order,
            request.Component.Description,
            request.Component.IsActive,
            userId);

        dbContext.Components.Update(component);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateComponentResult(component.Id, component.Name);
    }
}
