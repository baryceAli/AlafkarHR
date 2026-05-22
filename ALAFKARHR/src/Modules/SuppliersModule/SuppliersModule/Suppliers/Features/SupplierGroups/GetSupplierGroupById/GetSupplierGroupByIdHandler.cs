namespace SuppliersModule.Suppliers.Features.SupplierGroups.GetSupplierGroupById;

public record GetSupplierGroupByIdQuery(Guid Id) : IQuery<GetSupplierGroupByIdResult>;
public record GetSupplierGroupByIdResult(SupplierGroupDto SupplierGroup);

public class GetSupplierGroupByIdHandler(SupplierDbContext dbContext)
    : IQueryHandler<GetSupplierGroupByIdQuery, GetSupplierGroupByIdResult>
{
    public async Task<GetSupplierGroupByIdResult> Handle(GetSupplierGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var supplierGroup = await dbContext.SupplierGroups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        if (supplierGroup is null)
            throw new NotFoundException($"Supplier group not found: {request.Id}");

        return new GetSupplierGroupByIdResult(supplierGroup.Adapt<SupplierGroupDto>());
    }
}
