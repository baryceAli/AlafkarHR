

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
public class CreateWarehouseHandler (InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender): ICommandHandler<CreateWarehouseCommand, CreateWarehouseResult>
{
    public async Task<CreateWarehouseResult> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(c=> c.Type==ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User is not authenticated.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Warehouse.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, request.Warehouse.BranchId))
            throw new ForbiddenException("You do not have permission to create a warehouse in this branch scope.");

        var warehouse= Warehouse.Create(
            Guid.NewGuid(), 
            request.Warehouse.Name, 
            request.Warehouse.NameEng,
            request.Warehouse.Location,
            request.Warehouse.Address, 
            request.Warehouse.Longitude, 
            request.Warehouse.Latitude,
            request.Warehouse.CompanyId,
            request.Warehouse.BranchId,
            request.Warehouse.WarehouseType,
            userId);

        await dbContext.Warehouses.AddAsync(warehouse,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateWarehouseResult(warehouse.Id);
    }
}
