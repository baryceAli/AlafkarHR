using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Employees.ChangePosition;

public record ChangePositionCommand(ChangePositionDto ChangePosition) : ICommand<ChangePositionResult>;
public record ChangePositionResult(bool IsSuccess);
public class ChangePositionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ChangePositionCommand, ChangePositionResult>
{
    public async Task<ChangePositionResult> Handle(ChangePositionCommand request, CancellationToken cancellationToken)
    {
        var position= await dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p=>p.Id==request.ChangePosition.PositionId, cancellationToken);
        if (position is null)
            throw new NotFoundException($"Position not found: {request.ChangePosition.PositionId}");

        var employee=await dbContext.Employees.FirstOrDefaultAsync(e=> e.Id==request.ChangePosition.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.ChangePosition.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticate");

        employee.ChangePosition(request.ChangePosition.PositionId, userId);
        await dbContext.SaveChangesAsync();

        return new ChangePositionResult(true);
    }
}
