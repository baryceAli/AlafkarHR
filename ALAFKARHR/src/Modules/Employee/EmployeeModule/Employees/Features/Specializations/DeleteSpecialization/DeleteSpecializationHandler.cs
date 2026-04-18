using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Specializations.DeleteSpecialization;

public record DeleteSpecializationCommand(Guid Id) : ICommand<DeleteSpecializationResult>;
public record DeleteSpecializationResult(bool IsSuccess);
public class DeleteSpecializationHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteSpecializationCommand, DeleteSpecializationResult>
{
    public async Task<DeleteSpecializationResult> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var spec=await dbContext.Specializations.FirstOrDefaultAsync(s=> s.Id==request.Id, cancellationToken);
        if (spec is null)
            throw new NotFoundException($"Specialization not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        spec.Remove(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteSpecializationResult(true);
    }
}
