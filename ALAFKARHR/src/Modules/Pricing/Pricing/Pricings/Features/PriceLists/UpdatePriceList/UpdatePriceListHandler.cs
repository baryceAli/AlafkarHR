namespace Pricing.Pricings.Features.PriceLists.UpdatePriceList;

public record UpdatePriceListCommand(PriceListDto PriceList) : ICommand<UpdatePriceListResult>;
public record UpdatePriceListResult(bool IsSuccess);

public class UpdatePriceListHandler(PricingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdatePriceListCommand, UpdatePriceListResult>
{
    public async Task<UpdatePriceListResult> Handle(UpdatePriceListCommand command, CancellationToken cancellationToken)
    {
        var priceList = await dbContext.PriceLists.FirstOrDefaultAsync(p => p.Id == command.PriceList.Id, cancellationToken)
            ?? throw new NotFoundException($"Price list not found: {command.PriceList.Id}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        if (command.PriceList.IsDefault)
        {
            var defaultExists = await dbContext.PriceLists.AnyAsync(p =>
                p.Id != command.PriceList.Id &&
                p.CompanyId == priceList.CompanyId &&
                p.IsDefault &&
                p.IsActive, cancellationToken);

            if (defaultExists)
                throw new Exception("A default price list already exists for this company.");
        }

        priceList.Update(
            command.PriceList.Name,
            command.PriceList.Code,
            command.PriceList.CurrencyCode,
            command.PriceList.IsDefault,
            command.PriceList.IsActive,
            command.PriceList.EffectiveFrom,
            command.PriceList.EffectiveTo,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdatePriceListResult(true);
    }
}
