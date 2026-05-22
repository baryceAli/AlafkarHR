namespace Pricing.Pricings.Features.PriceListItems.UpdatePriceListItem;

public record UpdatePriceListItemCommand(PriceListItemDto PriceListItem) : ICommand<UpdatePriceListItemResult>;
public record UpdatePriceListItemResult(bool IsSuccess);

public class UpdatePriceListItemHandler(PricingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdatePriceListItemCommand, UpdatePriceListItemResult>
{
    public async Task<UpdatePriceListItemResult> Handle(UpdatePriceListItemCommand command, CancellationToken cancellationToken)
    {
        var item = await dbContext.PriceListItems.FirstOrDefaultAsync(i => i.Id == command.PriceListItem.Id, cancellationToken)
            ?? throw new NotFoundException($"Price list item not found: {command.PriceListItem.Id}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        item.Update(
            command.PriceListItem.ProductSkuId,
            command.PriceListItem.UnitId,
            command.PriceListItem.UnitPrice,
            command.PriceListItem.MinQuantity,
            command.PriceListItem.IsActive,
            command.PriceListItem.EffectiveFrom,
            command.PriceListItem.EffectiveTo,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdatePriceListItemResult(true);
    }
}
