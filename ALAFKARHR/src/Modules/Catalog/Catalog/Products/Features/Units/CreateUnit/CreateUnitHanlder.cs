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
        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?.Value??throw new UnauthorizedAccessException("User is not authenticated");

        var unit = Unit.Create(Guid.NewGuid(), command.Unit.UnitName, command.Unit.UnitNameEng, command.Unit.CompanyId.Value, userId);

        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUnitResult(unit.Id);

    }
}
