using Auth.Contracts.Features.RegisterUser;
using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using SharedWithUI.Auth.Dtos;
using SharedWithUI.General;
using System.Security.Claims;

namespace EmployeeModule.Employees.Features.Employees.CreateEmployee;

public record CreateEmployeeCommand(EmployeeDto Employee) : ICommand<CreateEmployeeResult>;
public record CreateEmployeeResult(EmployeeDto CreatedEmployee);
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(e=> e.Employee.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(e=> e.Employee.MiddleName).NotEmpty().WithMessage("MiddleName is required");
        RuleFor(e=> e.Employee.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(e=> e.Employee.Phone).NotEmpty().WithMessage("Phone is required");
        RuleFor(e=> e.Employee.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(e=> e.Employee.EmployeeNo).NotEmpty().WithMessage("EmployeeNo is required");
    }
}
public class CreateEmployeeHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateEmployeeCommand, CreateEmployeeResult>
{
    public async Task<CreateEmployeeResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        Random rnd = new Random();
        string code = rnd.Next(100, 9999).ToString();
        var employee = Employee.Create(
            Guid.NewGuid(),
            request.Employee.EmployeeNo,
            request.Employee.FirstName,
            request.Employee.MiddleName,
            request.Employee.LastName,
            request.Employee.Email,
            request.Employee.Phone,
            DateTimeToUTC.ToUtc(request.Employee.DateOfBirth),
            request.Employee.NationalId,
            DateTimeToUTC.ToUtc( request.Employee.HireDate),
            request.Employee.CompanyId!.Value,
            request.Employee.BranchId!.Value,
            request.Employee.AdministrationId!.Value,
            request.Employee.DepartmentId!.Value,
            request.Employee.PositionId!.Value,
            request.Employee.IdentityType,
            request.Employee.Gender,
            code,
            userId);
        await dbContext.Employees.AddAsync(employee, cancellationToken);

        


        await dbContext.SaveChangesAsync(cancellationToken);

        RegisterDto register = new RegisterDto(
                Guid.NewGuid(),
                request.Employee.EmployeeNo,
                request.Employee.Email,
                request.Employee.Phone,
                "User@123!",
                UserType.SystemUser,
                request.Employee.CompanyId.Value,
                employee.Id
            );

        var result = await sender.Send(new RegisterUserCommand(register));


        return new CreateEmployeeResult(employee.Adapt<EmployeeDto>());
    }
}
