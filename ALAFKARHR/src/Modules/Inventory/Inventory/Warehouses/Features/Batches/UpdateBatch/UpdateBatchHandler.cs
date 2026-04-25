namespace Inventory.Warehouses.Features.Batches.UpdateBatch;

public record UpdateBatchCommand(UpdateBatchDto Batch) : ICommand<UpdateBatchResult>;
public record UpdateBatchResult(bool IsSuccess);
public class UpdateBatchCommandValidator : AbstractValidator<UpdateBatchCommand>
{
    public UpdateBatchCommandValidator()
    {
        RuleFor(x => x.Batch).NotNull().WithMessage("Batch is required");
        RuleFor(x => x.Batch.Id).NotNull().WithMessage("Id is required");
        RuleFor(x => x.Batch.BatchNumber).NotEmpty().WithMessage("BatchNumber is required");
        RuleFor(x => x.Batch.ManufacturingDate).NotNull().WithMessage("ManufacturingDate is required");
        RuleFor(x => x.Batch.ExpiryDate).NotNull().WithMessage("ExpiryDate is required");
    }
}
public class UpdateBatchHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateBatchCommand, UpdateBatchResult>
{
    public async Task<UpdateBatchResult> Handle(UpdateBatchCommand command, CancellationToken cancellationToken)
    {
        var batch=await dbContext.Batches.FirstOrDefaultAsync(b=> b.Id==command.Batch.Id && b.DeletedAt==null,cancellationToken);

        if (batch is null)
            throw new Exception($"Batch not found: {command.Batch.Id}");
        
        var userId = httpContextAccessor.HttpContext?
              .User?
              .FindFirst(ClaimTypes.NameIdentifier)?
              .Value
              ?? throw new UnauthorizedAccessException("User not authenticated");

        batch.Update(command.Batch.BatchNumber,command.Batch.ManufacturingDate,command.Batch.ExpiryDate,userId);

        await dbContext.SaveChangesAsync();

        return new UpdateBatchResult(true);
    }
}
