using Inventory.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Warehouses.Features.Warehouses.GetWarehouseById;


public record GetWarehouseByIdQuery(Guid Id) : IQuery<GetWarehouseByIdResult>;
public record GetWarehouseByIdResult(WarehouseDto Warehouse);
public class GetWarehouseByIdHandler (InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor): IQueryHandler<GetWarehouseByIdQuery, GetWarehouseByIdResult>
{
    public async Task<GetWarehouseByIdResult> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse= await dbContext.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.DeletedAt==null, cancellationToken);

        if (warehouse is null)
            throw new Exception($"Warehous not found: {request.Id}");

        return new GetWarehouseByIdResult(warehouse.Adapt<WarehouseDto>());
    }
}
