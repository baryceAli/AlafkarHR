using FluentValidation;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Employees.TerminateEmployee;

public record TerminateEmployeeCommand(TerminateEmployeeDto TerminateEmployee) : ICommand<TerminateEmployeeResult>;
public record TerminateEmployeeResult(bool IsSuccess);

public class TerminateEmployeeCommandValidator : AbstractValidator<TerminateEmployeeCommand>
{
    public TerminateEmployeeCommandValidator()
    {
        RuleFor(x => x.TerminateEmployee).NotEmpty().WithMessage("Terminate Employee is required");
        RuleFor(x => x.TerminateEmployee.reason).NotEmpty().WithMessage("Reason is required");
    }
}
public class TerminateEmployeeHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<TerminateEmployeeCommand, TerminateEmployeeResult>
{
    public async Task<TerminateEmployeeResult> Handle(TerminateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.TerminateEmployee.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee is not found: {request.TerminateEmployee.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");
    
        employee.Terminate(request.TerminateEmployee.reason, userId);
        await dbContext.SaveChangesAsync();

        return new TerminateEmployeeResult(true);
    }


}
