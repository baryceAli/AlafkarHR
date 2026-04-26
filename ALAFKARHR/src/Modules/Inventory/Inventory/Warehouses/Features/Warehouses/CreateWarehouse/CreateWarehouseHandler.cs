

using Inventory.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inventory.Warehouses.Features.Warehouses.CreateWarehouse;

public record CreateWarehouseCommand(WarehouseDto Warehouse) : ICommand<CreateWarehouseResult>;
public record CreateWarehouseResult(Guid Id);

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseValidator()
    {
        RuleFor(x => x.Warehouse).NotNull().WithMessage("Warehouse Should not be null");
        RuleFor(x => x.Warehouse.Name).NotEmpty().MaximumLength(100).WithMessage("Name is required");
        RuleFor(x => x.Warehouse.NameEng).NotEmpty().MaximumLength(100).WithMessage("NameEng is required");
        RuleFor(x => x.Warehouse.Location).NotEmpty().MaximumLength(200).WithMessage("Location is required");
    }
}
public class CreateWarehouseHandler (InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor): ICommandHandler<CreateWarehouseCommand, CreateWarehouseResult>
{
    public async Task<CreateWarehouseResult> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(c=> c.Type==ClaimTypes.NameIdentifier)?.Value;
        var warehouse= Warehouse.Create(
            Guid.NewGuid(), 
            request.Warehouse.Name, 
            request.Warehouse.NameEng,
            request.Warehouse.Location,
            request.Warehouse.Address, 
            request.Warehouse.Longitude, 
            request.Warehouse.Latitude,
            request.Warehouse.CompanyId,
            userId);

        await dbContext.Warehouses.AddAsync(warehouse,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateWarehouseResult(warehouse.Id);
    }
}
