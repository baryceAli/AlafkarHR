using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Specializations.UpdateSpecialization;

public record UpdateSpecializationCommand(SpecializationDto Specialization) : ICommand<UpdateSpecializationResult>;
public record UpdateSpecializationResult(bool IsSuccess);
public class UpdateSpecializationHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSpecializationCommand, UpdateSpecializationResult>
{
    public async Task<UpdateSpecializationResult> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var spec = await dbContext.Specializations.FirstOrDefaultAsync(s => s.Id == request.Specialization.Id, cancellationToken);
        if (spec is null)
            throw new NotFoundException($"Specialization not found: {request.Specialization.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        spec.Update(request.Specialization.Name,
            request.Specialization.NameEng, userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateSpecializationResult(true);
    }
}
