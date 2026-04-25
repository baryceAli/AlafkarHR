using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Units.UpdateUnit;


public record UpdateUnitCommand(UnitDto Unit) : ICommand<UpdateUnitResult>;
public record UpdateUnitResult(bool IsSuccess);

public class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(x => x.Unit.UnitName).NotEmpty().WithMessage("UnitName is required");
        RuleFor(x => x.Unit.UnitNameEng).NotEmpty().WithMessage("UnitNameEng is required");
    }
}
public class UpdateUnitHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateUnitCommand, UpdateUnitResult>
{
    public async Task<UpdateUnitResult> Handle(UpdateUnitCommand command, CancellationToken cancellationToken)
    {
        var unit = await dbContext.Units.FindAsync([command.Unit.Id]);
        if (unit is null)
            throw new Exception($"Unit not found: {command.Unit.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        unit.Update(command.Unit.UnitName, command.Unit.UnitNameEng, userId);
        await dbContext.SaveChangesAsync();

        return new UpdateUnitResult(true);
    }
}
