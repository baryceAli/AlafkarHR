using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.CreateAcademicInstitution;

public record CreateAcademicInstitutionCommand(AcademicInstitutionDto AcademicInstitution) : ICommand<CreateAcademicInstitutionResult>;
public record CreateAcademicInstitutionResult(AcademicInstitutionDto CreatedAcademicInstitute);
public class CreateAcademicInstitutionCommandValidator:AbstractValidator<CreateAcademicInstitutionCommand>
{
    public CreateAcademicInstitutionCommandValidator()
    {
        RuleFor(a=> a.AcademicInstitution.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(a => a.AcademicInstitution.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class CreateAcademicInstitutionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateAcademicInstitutionCommand, CreateAcademicInstitutionResult>
{
    public async Task<CreateAcademicInstitutionResult> Handle(CreateAcademicInstitutionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var academicInstitute = Models.AcademicInstitution.Create(
            Guid.NewGuid(),
            request.AcademicInstitution.Name,
            request.AcademicInstitution.NameEng,
            userId);

        await dbContext.AcademicInstitutions.AddAsync(academicInstitute,cancellationToken);
        await dbContext.SaveChangesAsync();

        return new CreateAcademicInstitutionResult(academicInstitute.Adapt<AcademicInstitutionDto>());
    }
}
