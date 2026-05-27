namespace Pricing.Pricings.Features.PriceListItems.CreatePriceListItem;

public record CreatePriceListItemCommand(PriceListItemDto PriceListItem) : ICommand<CreatePriceListItemResult>;
public record CreatePriceListItemResult(Guid Id);

public class CreatePriceListItemValidator : AbstractValidator<CreatePriceListItemCommand>
{
    public CreatePriceListItemValidator()
    {
        RuleFor(x => x.PriceListItem.PriceListId).NotEmpty().WithMessage("PriceListId is required");
        RuleFor(x => x.PriceListItem.ProductSkuId).NotEmpty().WithMessage("ProductSkuId is required");
        RuleFor(x => x.PriceListItem.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");
    }
}

public class CreatePriceListItemHandler(PricingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreatePriceListItemCommand, CreatePriceListItemResult>
{
    public async Task<CreatePriceListItemResult> Handle(CreatePriceListItemCommand command, CancellationToken cancellationToken)
    {
        var priceListExists = await dbContext.PriceLists.AnyAsync(p => p.Id == command.PriceListItem.PriceListId, cancellationToken);
        if (!priceListExists)
            throw new NotFoundException($"Price list not found: {command.PriceListItem.PriceListId}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var item = PriceListItem.Create(
            Guid.NewGuid(),
            command.PriceListItem.PriceListId,
            command.PriceListItem.ProductSkuId,
            command.PriceListItem.UnitId,
            command.PriceListItem.UnitPrice,
            command.PriceListItem.MinQuantity,
            //command.PriceListItem.EffectiveFrom,
            //command.PriceListItem.EffectiveTo,
            user);

        await dbContext.PriceListItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatePriceListItemResult(item.Id);
    }
}
