
namespace Inventory.Warehouses.Features.Warehouses.RemoveWarehouse;

public record RemoveWarehouseCommand(Guid Id) : ICommand<RemoveWarehouseResult>;
public record RemoveWarehouseResult(bool IsSuccess);
public class RemoveWarehouseHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<RemoveWarehouseCommand, RemoveWarehouseResult>
{
    public async Task<RemoveWarehouseResult> Handle(RemoveWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse= dbContext.Warehouses.FirstOrDefault(w => w.Id == request.Id);
        if (warehouse is null)
            throw new Exception($"Warehouse not found: {request.Id}");

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
        warehouse.Remove(userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveWarehouseResult(true);
    }
}
