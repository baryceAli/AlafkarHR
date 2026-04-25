namespace Inventory.Warehouses.Features.Batches.RemoveBatch;

public record RemoveBatchCommand(Guid Id) : ICommand<RemoveBatchResult>;
public record RemoveBatchResult(bool IsSuccess);

public class RemoveBatchHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<RemoveBatchCommand, RemoveBatchResult>
{
    public async Task<RemoveBatchResult> Handle(RemoveBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await dbContext.Batches.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (batch is null)
            throw new Exception($"Batch not found: {request.Id}");

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        batch.Remove(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveBatchResult(true);
    }
}
