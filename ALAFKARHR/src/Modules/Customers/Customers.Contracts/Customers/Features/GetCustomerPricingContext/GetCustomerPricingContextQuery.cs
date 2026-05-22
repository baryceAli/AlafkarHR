using Shared.Contracts.CQRS;

namespace Customers.Contracts.Customers.Features.GetCustomerPricingContext;

public record GetCustomerPricingContextQuery(Guid CustomerId, Guid CompanyId, DateTime PriceDate)
    : IQuery<GetCustomerPricingContextResult>;

public record GetCustomerPricingContextResult(
    Guid CustomerId,
    Guid? CustomerGroupId,
    bool IsTaxExempt,
    Guid? ProfilePriceListId,
    decimal? ProfileDiscountPercentage,
    Guid? GroupDefaultPriceListId,
    decimal? GroupDefaultDiscountPercentage);
