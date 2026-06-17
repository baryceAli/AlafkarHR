using Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;

namespace CustomersModule.Customers.Features.Customers.GetCustomerSalesEligibility;

public class GetCustomerSalesEligibilityHandler(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerSalesEligibilityQuery, GetCustomerSalesEligibilityResult>
{
    public async Task<GetCustomerSalesEligibilityResult> Handle(GetCustomerSalesEligibilityQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CustomerId && x.CompanyId == request.CompanyId, cancellationToken);

        if (customer is null)
        {
            return new GetCustomerSalesEligibilityResult(
                request.CustomerId,
                request.CompanyId,
                false,
                false,
                false,
                0m,
                0m,
                false,
                null,
                "Customer not found.");
        }

        var isActive = customer.Status == CustomerStatus.Active;
        var creditAllowed = customer.CreditStatus == CreditStatus.Good && customer.AvailableCredit >= request.RequestedAmount;
        var blockReason = !isActive
            ? "Customer is not active."
            : !creditAllowed
                ? customer.CreditHoldReason ?? "Customer credit is not sufficient or is on hold."
                : null;

        return new GetCustomerSalesEligibilityResult(
            customer.Id,
            customer.CompanyId,
            true,
            isActive,
            creditAllowed,
            customer.CreditLimit,
            customer.AvailableCredit,
            customer.IsTaxExempt,
            customer.CustomerGroupId,
            blockReason);
    }
}
