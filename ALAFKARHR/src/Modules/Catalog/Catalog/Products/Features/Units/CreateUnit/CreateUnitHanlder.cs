namespace Catalog.Products.Features.Units.CreateUnit;

public record CreateUnitCommand(UnitDto Unit) : ICommand<CreateUnitResult>;
public record CreateUnitResult(Guid Id);

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(x => x.Unit.UnitName).NotEmpty().WithMessage("UnitName is required");
        RuleFor(x => x.Unit.UnitNameEng).NotEmpty().WithMessage("UnitNameEng is required");
    }
}

public class CreateUnitHanlder(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateUnitCommand, CreateUnitResult>
{
    public async Task<CreateUnitResult> Handle(CreateUnitCommand command, CancellationToken cancellationToken)
    {
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var unit = Unit.Create(Guid.NewGuid(), command.Unit.UnitName, command.Unit.UnitNameEng, Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), userId);

        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUnitResult(unit.Id);

    }
}
