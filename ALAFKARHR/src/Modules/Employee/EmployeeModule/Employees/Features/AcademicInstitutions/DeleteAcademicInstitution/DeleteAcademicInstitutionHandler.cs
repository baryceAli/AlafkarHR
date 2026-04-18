using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.DeleteAcademicInstitution;

public record DeleteAcademicInstitutionCommand(Guid Id) : ICommand<DeleteAcademicInstitutionResult>;
public record DeleteAcademicInstitutionResult(bool IsSuccess);
public class DeleteAcademicInstitutionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteAcademicInstitutionCommand, DeleteAcademicInstitutionResult>
{
    public async Task<DeleteAcademicInstitutionResult> Handle(DeleteAcademicInstitutionCommand request, CancellationToken cancellationToken)
    {
        var academic=await dbContext.AcademicInstitutions.FirstOrDefaultAsync(a=> a.Id==request.Id, cancellationToken);
        if (academic is null)
            throw new NotFoundException($"Academic Institution not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        academic.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new DeleteAcademicInstitutionResult(true);

    }
}
