using Shared.Pagination;

namespace SuppliersModule.Suppliers.Features.Suppliers.GetSuppliersByCompanyId;

public record GetSuppliersByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetSuppliersByCompanyIdResult>;
public record GetSuppliersByCompanyIdResult(PaginatedResult<SupplierDto> SupplierList);

public class GetSuppliersByCompanyIdHandler(SupplierDbContext dbContext)
    : IQueryHandler<GetSuppliersByCompanyIdQuery, GetSuppliersByCompanyIdResult>
{
    public async Task<GetSuppliersByCompanyIdResult> Handle(GetSuppliersByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Suppliers.AsNoTracking().Where(s => s.CompanyId == request.CompanyId);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var lowerSearch = searchText.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(lowerSearch) ||
                s.SupplierCode.ToLower().Contains(lowerSearch) ||
                (s.CommercialName != null && s.CommercialName.ToLower().Contains(lowerSearch)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var suppliers = await query
            .Include(s => s.Addresses)
            .Include(s => s.Contacts)
            .OrderBy(s => s.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetSuppliersByCompanyIdResult(new PaginatedResult<SupplierDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            suppliers.Adapt<List<SupplierDto>>()));
    }
}
