using Shared.Contracts.CQRS;

namespace Customers.Contracts.Customers.Features.ValidateCustomerAddresses;

public record ValidateCustomerAddressesQuery(Guid CustomerId, Guid CompanyId, IReadOnlyCollection<Guid> AddressIds)
    : IQuery<ValidateCustomerAddressesResult>;

public record ValidateCustomerAddressesResult(bool IsValid, IReadOnlyCollection<Guid> InvalidAddressIds);
