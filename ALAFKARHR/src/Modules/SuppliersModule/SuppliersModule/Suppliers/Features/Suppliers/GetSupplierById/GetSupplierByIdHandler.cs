namespace SuppliersModule.Suppliers.Features.Suppliers.GetSupplierById;

public record GetSupplierByIdQuery(Guid Id) : IQuery<GetSupplierByIdResult>;
public record GetSupplierByIdResult(SupplierDto Supplier);

public class GetSupplierByIdHandler(SupplierDbContext dbContext)
    : IQueryHandler<GetSupplierByIdQuery, GetSupplierByIdResult>
{
    public async Task<GetSupplierByIdResult> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await dbContext.Suppliers
            .Include(s => s.Addresses)
            .Include(s => s.Contacts)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Supplier not found: {request.Id}");

        return new GetSupplierByIdResult(supplier.Adapt<SupplierDto>());
    }
}
