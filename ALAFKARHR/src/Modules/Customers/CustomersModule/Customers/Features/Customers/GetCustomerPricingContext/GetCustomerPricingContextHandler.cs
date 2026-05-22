using Customers.Contracts.Customers.Features.GetCustomerPricingContext;

namespace CustomersModule.Customers.Features.Customers.GetCustomerPricingContext;

public class GetCustomerPricingContextHandler(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerPricingContextQuery, GetCustomerPricingContextResult>
{
    public async Task<GetCustomerPricingContextResult> Handle(GetCustomerPricingContextQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.CompanyId == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException($"Customer not found: {request.CustomerId}");

        var customerGroup = customer.CustomerGroupId.HasValue
            ? await dbContext.CustomerGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == customer.CustomerGroupId.Value && g.CompanyId == request.CompanyId, cancellationToken)
            : null;

        var profile = await dbContext.CustomerPricingProfiles
            .AsNoTracking()
            .Where(p => p.CustomerId == request.CustomerId &&
                        p.CompanyId == request.CompanyId &&
                        p.EffectiveFrom <= request.PriceDate &&
                        (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= request.PriceDate))
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetCustomerPricingContextResult(
            customer.Id,
            customer.CustomerGroupId,
            customer.IsTaxExempt,
            profile?.PriceListId,
            profile?.DiscountPercentage,
            customerGroup?.DefaultPriceListId,
            customerGroup?.DefaultDiscountPercentage);
    }
}
