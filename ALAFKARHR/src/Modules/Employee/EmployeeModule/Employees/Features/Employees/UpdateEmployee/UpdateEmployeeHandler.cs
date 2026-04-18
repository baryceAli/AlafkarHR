using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Employees.UpdateEmployee;

public record UpdateEmployeeCommand(EmployeeDto Employee) : ICommand<UpdateEmployeeResult>;
public record UpdateEmployeeResult(bool IsSuccess);

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(e=> e.Employee.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(e=> e.Employee.MiddleName).NotEmpty().WithMessage("MiddleName is required");
        RuleFor(e=> e.Employee.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(e=> e.Employee.Phone).NotEmpty().WithMessage("Phone is required");
        RuleFor(e=> e.Employee.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(e=> e.Employee.HireDate).NotEmpty().WithMessage("HireDate is required");
        RuleFor(e=> e.Employee.DateOfBirth).NotEmpty().WithMessage("DateOfBirth is required");
        RuleFor(e=> e.Employee.EmployeeNo).NotEmpty().WithMessage("EmployeeNo is required");
        RuleFor(e=> e.Employee.NationalId).NotEmpty().WithMessage("NationalId is required");
    }
}
public class UpdateEmployeeHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResult>
{
    public async Task<UpdateEmployeeResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var position = await dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Employee.PositionId, cancellationToken);
        if (position is null)
            throw new NotFoundException($"Position not found: {request.Employee.PositionId}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");
        var employee=await dbContext.Employees.FirstOrDefaultAsync(e=> e.Id==request.Employee.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.Employee.Id}");


        
        employee.Update(
            request.Employee.FirstName,
            request.Employee.FirstNameEng,
            request.Employee.MiddleName,
            request.Employee.MiddleNameEng,
            request.Employee.LastName,
            request.Employee.LastNameEng,
            request.Employee.Email,
            request.Employee.Phone,
            request.Employee.Address,
            request.Employee.MaritalStatus,
            request.Employee.EmploymentType,
            request.Employee.Qualification,
            request.Employee.SpecializationId,
            request.Employee.AcademicInstituteId,
            request.Employee.GraduationYear,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateEmployeeResult(true);
    }
}
