namespace Catalog.Products.Features.Units.CreateUnit;

public record CreateUnitCommand(UnitDto Unit) : ICommand<CreateUnitResult>;
public record CreateUnitResult(Guid Id);

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(x => x.Unit.UnitName).NotEmpty().WithMessage("UnitName is required");
        RuleFor(x => x.Unit.UnitNameEng).NotEmpty().WithMessage("UnitNameEng is required");
        RuleFor(x => x.Unit.UnitCategory).NotEmpty().WithMessage("Unit category is required");
        RuleFor(x => x.Unit.ConversionFactor).GreaterThan(0).WithMessage("Conversion factor must be greater than 0");
    }
}

public class CreateUnitHanlder(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateUnitCommand, CreateUnitResult>
{
    public async Task<CreateUnitResult> Handle(CreateUnitCommand command, CancellationToken cancellationToken)
    {
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        await EnsureSingleReferenceUnitAsync(command.Unit, companyId, cancellationToken);

        var unit = Unit.Create(
            Guid.NewGuid(),
            command.Unit.UnitName,
            command.Unit.UnitNameEng,
            command.Unit.UnitCategory,
            command.Unit.ConversionFactor,
            command.Unit.IsReferenceUnit,
            companyId,
            userId);

        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUnitResult(unit.Id);

    }

    private async Task EnsureSingleReferenceUnitAsync(UnitDto unit, Guid companyId, CancellationToken cancellationToken)
    {
        if (!unit.IsReferenceUnit)
            return;

        var category = string.IsNullOrWhiteSpace(unit.UnitCategory) ? "General" : unit.UnitCategory.Trim();
        var exists = await dbContext.Units.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.UnitCategory == category && x.IsReferenceUnit, cancellationToken);

        if (exists)
            throw new Exception($"Reference unit already exists for category: {category}");
    }
}
