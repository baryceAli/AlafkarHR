using Customers.Contracts.Customers.Features.ValidateCustomerAddresses;
using CustomersModule.Data;
using Shared.Contracts.CQRS;

namespace CustomersModule.Customers.Features.Customers.ValidateCustomerAddresses;

public class ValidateCustomerAddressesHandler(CustomerDbContext dbContext)
    : IQueryHandler<ValidateCustomerAddressesQuery, ValidateCustomerAddressesResult>
{
    public async Task<ValidateCustomerAddressesResult> Handle(ValidateCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        var requestedIds = request.AddressIds.Where(x => x != Guid.Empty).Distinct().ToList();
        if (requestedIds.Count == 0)
            return new ValidateCustomerAddressesResult(true, []);

        var customer = await dbContext.Customers
            .Include(x => x.Addresses)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CustomerId && x.CompanyId == request.CompanyId, cancellationToken);

        if (customer is null)
            return new ValidateCustomerAddressesResult(false, requestedIds);

        var validIds = customer.Addresses
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToHashSet();

        var invalidIds = requestedIds.Where(x => !validIds.Contains(x)).ToList();
        return new ValidateCustomerAddressesResult(invalidIds.Count == 0, invalidIds);
    }
}
