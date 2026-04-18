using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;

namespace EmployeeModule.Employees.Features.Positions.UpdatePosition;


public record UpdatePositionCommand(PositionDto Position):ICommand<UpdatePositionResult>;
public record UpdatePositionResult(bool IsSuccess);
public class UpdatePositionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdatePositionCommand, UpdatePositionResult>
{
    public async Task<UpdatePositionResult> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.Position.Id, cancellationToken);
        if (position is null)
            throw new NotFoundException($"Position not found: {request.Position.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        position.Update(request.Position.Title,
                        request.Position.TitleEng,
                        request.Position.BaseSalary,
                        userId);

        await dbContext.SaveChangesAsync();

        return new UpdatePositionResult(true);
    }
}
