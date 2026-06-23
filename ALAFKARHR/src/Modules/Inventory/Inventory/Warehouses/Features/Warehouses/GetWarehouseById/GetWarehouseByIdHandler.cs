using Inventory.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Warehouses.Features.Warehouses.GetWarehouseById;


public record GetWarehouseByIdQuery(Guid Id) : IQuery<GetWarehouseByIdResult>;
public record GetWarehouseByIdResult(WarehouseDto Warehouse);
public class GetWarehouseByIdHandler (InventoryDbContext dbContext, ISender sender): IQueryHandler<GetWarehouseByIdQuery, GetWarehouseByIdResult>
{
    public async Task<GetWarehouseByIdResult> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse= await dbContext.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.DeletedAt==null, cancellationToken);

        if (warehouse is null)
            throw new Exception($"Warehous not found: {request.Id}");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(warehouse.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, warehouse.BranchId))
            throw new ForbiddenException("You do not have permission to view this warehouse.");

        return new GetWarehouseByIdResult(warehouse.Adapt<WarehouseDto>());
    }
}
