using Catalog.Contracts.Products.Features.GetProductByCompany;

namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventoriesByCompany;

public record GetInventoriesByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetInventoriesByCompanyResult>;
public record GetInventoriesByCompanyResult(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesByCompanyHandler(InventoryDbContext dbContext, ISender sender) : IQueryHandler<GetInventoriesByCompanyQuery, GetInventoriesByCompanyResult>
{
    public async Task<GetInventoriesByCompanyResult> Handle(GetInventoriesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var q = dbContext.Inventories
            .Include(x=>x.Batches)
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        
        
        var prodRes = await sender.Send(new GetProductByCompanyQuery(request.CompanyId, new PaginationRequest(0, int.MaxValue)), cancellationToken);
        var products = prodRes.ProductList.Data.ToDictionary(x => x.Id, x => x);
        var data = await q
            .ToListAsync();

        var warehouses=await dbContext.Warehouses.AsNoTracking().Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId).ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
        var query= data.Select(x =>
        {
            var dto = x.Adapt<InventoryAggregateDto>();
            if (products.TryGetValue(x.ProductId, out var prod))
            {
                dto.ProductName = prod.Name;
                dto.ProductNameEng = prod.NameEng;
                var sku = prod.Skus.FirstOrDefault(s => s.Id == x.ProductSkuId);
                if (sku != null)
                {
                    dto.ProductSkuName = sku.Name;
                    dto.ProductSkuNameEng = sku.NameEng;
                }

            }
            if (warehouses.TryGetValue(x.WarehouseId, out var warehouse))
            {
                dto.WarehouseName = warehouse.Name;
                dto.WarehouseNameEng = warehouse.NameEng;
            }
            dto.TotalQuantity = x.TotalQuantity;
            dto.TotalReserved = x.TotalReserved;
            dto.TotalAvailable = x.TotalAvailable;
            return dto;
        }).AsQueryable();
        //.ToList();

        string? searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(x => 
                            x.ProductName.Contains(searchText) || 
                            x.ProductNameEng.Contains(searchText) || 
                            x.ProductSkuName.Contains(searchText) || 
                            x.ProductSkuNameEng.Contains(searchText) || 
                            x.WarehouseName.Contains(searchText) || 
                            x.WarehouseNameEng.Contains(searchText));
        }

        var count = query.LongCount();

        var inventoryDtos = query
                            .OrderByDescending(x => x.CreatedAt)
                            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                            .Take(request.PaginationRequest.PageSize)
                            .ToList();
        return new GetInventoriesByCompanyResult(
            new PaginatedResult<InventoryAggregateDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, inventoryDtos));




    }
}
