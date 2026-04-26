namespace Inventory.Warehouses.Features.Batches.CreateBatch;

public record CreateBatchCommand(CreateBatchDto Batch) : ICommand<CreateBatchResult>;
public record CreateBatchResult(Guid Id);

public class CreateBatchCommandValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchCommandValidator()
    {
        RuleFor(x => x.Batch.BatchNumber).NotEmpty().WithMessage("BatchNumber is required");
        //RuleFor(x => x.Batch.WarehouseId).NotNull().WithMessage("WarehouseId is required");
        RuleFor(x => x.Batch.ProductId).NotNull().WithMessage("ProductId is required");
        RuleFor(x => x.Batch.ProductSkuId).NotNull().WithMessage("ProductSkuId is required");
        RuleFor(x => x.Batch.ManufacturingDate).NotNull().WithMessage("ManufacturingDate is required");
        RuleFor(x => x.Batch.ExpiryDate).NotNull().WithMessage("ExpiryDate is required");
    }
}
public class CreateBatchHandler (InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateBatchCommand, CreateBatchResult>
{
    public async Task<CreateBatchResult> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Batches
            .AnyAsync(b => b.ProductSkuId == request.Batch.ProductId
                        //&& b.WarehouseId == request.Batch.WarehouseId
                        && b.BatchNumber == request.Batch.BatchNumber);

        if (exists)
            throw new Exception("Batch already exists");

        var userId = httpContextAccessor.HttpContext?.User?
            .FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value??
            throw new Exception("No valid logged in user");

        var batch = Batch.Create(Guid.NewGuid(),
                        //request.Batch.WarehouseId,
                        request.Batch.ProductId,
                        request.Batch.ProductId,
                        request.Batch.BatchNumber,
                        request.Batch.ManufacturingDate,
                        request.Batch.ExpiryDate,
                        request.Batch.CompanyId.Value,
                        userId);

        await dbContext.Batches.AddAsync(batch, cancellationToken);
        await dbContext.SaveChangesAsync();
        return new CreateBatchResult(batch.Id);
    }
}
