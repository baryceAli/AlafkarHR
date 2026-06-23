
namespace Inventory.Warehouses.Features.Warehouses.RemoveWarehouse;

public record RemoveWarehouseCommand(Guid Id) : ICommand<RemoveWarehouseResult>;
public record RemoveWarehouseResult(bool IsSuccess);
public class RemoveWarehouseHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender) : ICommandHandler<RemoveWarehouseCommand, RemoveWarehouseResult>
{
    public async Task<RemoveWarehouseResult> Handle(RemoveWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse= await dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);
        if (warehouse is null)
            throw new Exception($"Warehouse not found: {request.Id}");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(warehouse.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, warehouse.BranchId))
            throw new ForbiddenException("You do not have permission to delete this warehouse.");

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User is not authenticated.");

        warehouse.Remove(userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveWarehouseResult(true);
    }
}
