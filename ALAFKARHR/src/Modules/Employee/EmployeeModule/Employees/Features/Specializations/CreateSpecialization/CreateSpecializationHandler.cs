using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Specializations.CreateSpecialization;

public record CreateSpecializationCommand(SpecializationDto Specialization) : ICommand<CreateSpecializationResult>;
public record CreateSpecializationResult(SpecializationDto CreatedSpecialization);

public class CreateSpecializationCommandValidator : AbstractValidator<CreateSpecializationCommand>
{
    public CreateSpecializationCommandValidator()
    {
        RuleFor(s=> s.Specialization.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(s => s.Specialization.NameEng).NotEmpty().WithMessage("NameEnd is required");
    }
}
public class CreateSpecializationHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSpecializationCommand, CreateSpecializationResult>
{
    public async Task<CreateSpecializationResult> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var specialization=Specialization.Create(
                                    Guid.NewGuid(),
                                    request.Specialization.Name,
                                    request.Specialization.NameEng,
                                    request.Specialization.CompanyId,
                                    userId);

        await dbContext.Specializations.AddAsync(specialization, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSpecializationResult(specialization.Adapt<SpecializationDto>());
    }
}
