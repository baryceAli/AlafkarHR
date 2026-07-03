namespace Catalog.Products.Features.Units.UpdateUnit;


public record UpdateUnitCommand(UnitDto Unit) : ICommand<UpdateUnitResult>;
public record UpdateUnitResult(bool IsSuccess);

public class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(x => x.Unit.UnitName).NotEmpty().WithMessage("UnitName is required");
        RuleFor(x => x.Unit.UnitNameEng).NotEmpty().WithMessage("UnitNameEng is required");
        RuleFor(x => x.Unit.UnitCategory).NotEmpty().WithMessage("Unit category is required");
        RuleFor(x => x.Unit.ConversionFactor).GreaterThan(0).WithMessage("Conversion factor must be greater than 0");
    }
}
public class UpdateUnitHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateUnitCommand, UpdateUnitResult>
{
    public async Task<UpdateUnitResult> Handle(UpdateUnitCommand command, CancellationToken cancellationToken)
    {
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        var unit = await dbContext.Units.FirstOrDefaultAsync(x => x.Id == command.Unit.Id && x.CompanyId == companyId, cancellationToken);
        if (unit is null)
            throw new Exception($"Unit not found: {command.Unit.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        await EnsureSingleReferenceUnitAsync(command.Unit, companyId, cancellationToken);

        unit.Update(
            command.Unit.UnitName,
            command.Unit.UnitNameEng,
            command.Unit.UnitCategory,
            command.Unit.ConversionFactor,
            command.Unit.IsReferenceUnit,
            userId);
        await dbContext.SaveChangesAsync();

        return new UpdateUnitResult(true);
    }

    private async Task EnsureSingleReferenceUnitAsync(UnitDto unit, Guid companyId, CancellationToken cancellationToken)
    {
        if (!unit.IsReferenceUnit)
            return;

        var category = string.IsNullOrWhiteSpace(unit.UnitCategory) ? "General" : unit.UnitCategory.Trim();
        var exists = await dbContext.Units.AsNoTracking()
            .AnyAsync(x => x.Id != unit.Id && x.CompanyId == companyId && x.UnitCategory == category && x.IsReferenceUnit, cancellationToken);

        if (exists)
            throw new Exception($"Reference unit already exists for category: {category}");
    }
}
