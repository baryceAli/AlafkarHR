using Shared.Contracts.CQRS;

namespace Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;

public record GetCustomerSalesEligibilityQuery(Guid CustomerId, Guid CompanyId, decimal RequestedAmount)
    : IQuery<GetCustomerSalesEligibilityResult>;

public record GetCustomerSalesEligibilityResult(
    Guid CustomerId,
    Guid CompanyId,
    bool Exists,
    bool IsActive,
    bool IsCreditAllowed,
    decimal CreditLimit,
    decimal AvailableCredit,
    bool IsTaxExempt,
    Guid? CustomerGroupId,
    string? BlockReason);
