using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.UpdateAcademicInstitution;

public record UpdateAcademicInstitutionCommand(AcademicInstitutionDto AcademicInstitution) : ICommand<UpdateAcademicInstitutionResult>;
public record UpdateAcademicInstitutionResult(bool IsSuccess);
public class UpdateAcademicInstitutionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateAcademicInstitutionCommand, UpdateAcademicInstitutionResult>
{
    public async Task<UpdateAcademicInstitutionResult> Handle(UpdateAcademicInstitutionCommand request, CancellationToken cancellationToken)
    {
        var academic = await dbContext.AcademicInstitutions.FirstOrDefaultAsync(a => a.Id == request.AcademicInstitution.Id, cancellationToken);
        if (academic is null)
            throw new NotFoundException($"Academic Institution not found: {request.AcademicInstitution.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        academic.Update(
            request.AcademicInstitution.Name,
            request.AcademicInstitution.NameEng, userId);
    
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateAcademicInstitutionResult(true);
    
    }
}
