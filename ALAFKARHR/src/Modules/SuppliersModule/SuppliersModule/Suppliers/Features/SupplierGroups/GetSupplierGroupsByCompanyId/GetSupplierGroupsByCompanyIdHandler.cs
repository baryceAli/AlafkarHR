namespace SuppliersModule.Suppliers.Features.SupplierGroups.GetSupplierGroupsByCompanyId;

public record GetSupplierGroupsByCompanyIdQuery(Guid CompanyId) : IQuery<GetSupplierGroupsByCompanyIdResult>;
public record GetSupplierGroupsByCompanyIdResult(List<SupplierGroupDto> SupplierGroups);

public class GetSupplierGroupsByCompanyIdHandler(SupplierDbContext dbContext)
    : IQueryHandler<GetSupplierGroupsByCompanyIdQuery, GetSupplierGroupsByCompanyIdResult>
{
    public async Task<GetSupplierGroupsByCompanyIdResult> Handle(GetSupplierGroupsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var supplierGroups = await dbContext.SupplierGroups
            .AsNoTracking()
            .Where(g => g.CompanyId == request.CompanyId)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);

        return new GetSupplierGroupsByCompanyIdResult(supplierGroups.Adapt<List<SupplierGroupDto>>());
    }
}
