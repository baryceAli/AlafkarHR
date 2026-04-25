using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Units.RemoveUnit;

public record RemoveUnitCommand(Guid Id) : ICommand<RemoveUnitResult>;
public record RemoveUnitResult(bool IsSuccess);

public class RemoveUnitHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveUnitCommand, RemoveUnitResult>
{
    public async Task<RemoveUnitResult> Handle(RemoveUnitCommand request, CancellationToken cancellationToken)
    {
        var unit=await dbContext.Units.FindAsync([request.Id]);
        if ((unit is null))
            throw new Exception($"Unit not found: {request.Id}");
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        unit.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new RemoveUnitResult(true);
    }
}
