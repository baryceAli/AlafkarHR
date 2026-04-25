namespace Inventory.Warehouses.Features.Warehouses.UpdateWarehouse;

public record UpdateWarehouseCommand(WarehouseDto Warehouse) : ICommand<UpdateWarehouseResult>;
public record UpdateWarehouseResult(bool IsSuccess);
public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.Warehouse).NotNull().WithMessage("Warehouse should not be null");
        RuleFor(x => x.Warehouse.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Warehouse.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Warehouse.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Warehouse.Location).NotEmpty().WithMessage("Location is required");
    }
}
public class UpdateWarehouseHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateWarehouseCommand, UpdateWarehouseResult>
{
    public async Task<UpdateWarehouseResult> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses.FindAsync(new object[] { request.Warehouse.Id }, cancellationToken);
        if (warehouse is null)
            throw new Exception($"Warehouse not found: {request.Warehouse.Id}");

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
        warehouse.Update(
            request.Warehouse.Name, 
            request.Warehouse.NameEng,
            request.Warehouse.Location,
            request.Warehouse.Address, 
            request.Warehouse.Longitude, 
            request.Warehouse.Latitude, userId);
        await dbContext.SaveChangesAsync();
        return new UpdateWarehouseResult(true);
    }
}
