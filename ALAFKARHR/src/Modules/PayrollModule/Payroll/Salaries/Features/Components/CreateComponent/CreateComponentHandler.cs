namespace Payroll.Salaries.Features.Components.CreateComponent;

public class CreateComponentHandler(PayrollDbContext dbContext)
    : ICommandHandler<CreateComponentCommand, CreateComponentResult>
{
    public async Task<CreateComponentResult> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
    {
        var component = Component.Create(
            Guid.NewGuid(),
            request.Component.Name,
            request.Component.NameEng,
            request.Component.ComponentType,
            request.Component.IsTaxable,
            request.Component.Order,
            request.Component.Description,
            request.Component.CompanyId);

        await dbContext.Components.AddAsync(component, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateComponentResult(component.Id, component.Name);
    }
}
