namespace Pricing.Pricings.Features.PriceLists.CreatePriceList;

public record CreatePriceListCommand(PriceListDto PriceList) : ICommand<CreatePriceListResult>;
public record CreatePriceListResult(Guid Id);

public class CreatePriceListValidator : AbstractValidator<CreatePriceListCommand>
{
    public CreatePriceListValidator()
    {
        RuleFor(x => x.PriceList.Name).NotEmpty().WithMessage("Name is required");
        //RuleFor(x => x.PriceList.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.PriceList.CompanyId).NotEmpty().WithMessage("Company is required");
        RuleFor(x => x.PriceList.CurrencyId).NotEmpty().WithMessage("Currency is required");
    }
}

public class CreatePriceListHandler(PricingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreatePriceListCommand, CreatePriceListResult>
{
    public async Task<CreatePriceListResult> Handle(CreatePriceListCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        if (command.PriceList.IsDefault)
        {
            var defaultExists = await dbContext.PriceLists.AnyAsync(p =>
                p.CompanyId == command.PriceList.CompanyId &&
                p.IsDefault &&
                p.IsActive, cancellationToken);

            if (defaultExists)
                throw new Exception("A default price list already exists for this company.");
        }
        var code=await GeneratePriceListCode(command.PriceList.CompanyId, cancellationToken);

        var priceList = PriceList.Create(
            Guid.NewGuid(),
            command.PriceList.Name,
            code,
            command.PriceList.CompanyId,
            command.PriceList.CurrencyId!.Value,
            command.PriceList.CurrencyCode,
            command.PriceList.IsDefault,
            command.PriceList.EffectiveFrom,
            command.PriceList.EffectiveTo,
            user);

        if (command.PriceList.Items.Any())
        {
            foreach(var pl in command.PriceList.Items)
            {
                priceList.AddPriceListItem(pl.ProductSkuId, pl.UnitId, pl.UnitPrice, pl.MinQuantity, user);
            }
        }

        await dbContext.PriceLists.AddAsync(priceList, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatePriceListResult(priceList.Id);
    }
    private async Task<string> GeneratePriceListCode(Guid companyId, CancellationToken cancellationToken)
    {
        var count = await dbContext.PriceLists.IgnoreQueryFilters().LongCountAsync(s => s.CompanyId == companyId, cancellationToken);
        return $"PL-{count + 1:00000}";
    }
}
