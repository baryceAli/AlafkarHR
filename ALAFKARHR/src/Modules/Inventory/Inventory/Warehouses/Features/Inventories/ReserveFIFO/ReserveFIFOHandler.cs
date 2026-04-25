namespace Inventory.Warehouses.Features.Inventories.ReserveFIFO;

public record ReserveFIFOCommand(Guid InventoryId, decimal Quantity, List<(Guid BatchId, DateTime ExpiryDate)> BatchExpiries) : ICommand<ReserveFIFOResult>;
public record ReserveFIFOResult(List<ReserveQuantityDto> Allocations);

public class ReserveFIFOCommandValidator : AbstractValidator<ReserveFIFOCommand>
{
    public ReserveFIFOCommandValidator()
    {
        RuleFor(x => x.InventoryId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class ReserveFIFOHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<ReserveFIFOCommand, ReserveFIFOResult>
{
    public async Task<ReserveFIFOResult> Handle(ReserveFIFOCommand request, CancellationToken cancellationToken)
    {
        var inventory = await dbContext.Inventories.Include("_batches")
            .FirstOrDefaultAsync(i => i.Id == request.InventoryId, cancellationToken);

        if (inventory is null)
            throw new Exception($"Inventory not found: {request.InventoryId}");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var allocations = inventory.ReserveFIFO(request.Quantity, request.BatchExpiries, userId);

        // map to InventoryReserve DTO
        var resultAllocations = allocations
            .Select(a => 
            new ReserveQuantityDto(
                Guid.Empty,
                Guid.Empty, 
                Guid.Empty,
                Guid.Empty,
                a.BatchId, 
                a.Quantity))
            .ToList();

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ReserveFIFOResult(resultAllocations);
    }
}
