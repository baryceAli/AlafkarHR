using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Positions.DeletePosition;


public record DeletePositionCommand(Guid Id) : ICommand<DeletePositionResult>;
public record DeletePositionResult(bool IsSuccess);
public class DeletePositionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeletePositionCommand, DeletePositionResult>
{
    public async Task<DeletePositionResult> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position=await dbContext.Positions.FirstOrDefaultAsync(p=> p.Id==request.Id, cancellationToken);
        if (position == null)
            throw new NotFoundException($"Position not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        position.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new DeletePositionResult(true);
    }
}
