using Inventory.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Warehouses.Features.Batches.GetBatchById;

public record GetBatchByIdQuery(Guid Id) : IQuery<GetBatchByIdResult>;
public record GetBatchByIdResult(BatchDto Batch);
public class GetBatchByIdHandler(InventoryDbContext dbContext) : IQueryHandler<GetBatchByIdQuery, GetBatchByIdResult>
{
    public async Task<GetBatchByIdResult> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = await dbContext.Batches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.DeletedAt == null, cancellationToken);

        if (batch is null)
            throw new Exception($"Batch not found: {request.Id}");

        return new GetBatchByIdResult(batch.Adapt<BatchDto>());
    }
}
