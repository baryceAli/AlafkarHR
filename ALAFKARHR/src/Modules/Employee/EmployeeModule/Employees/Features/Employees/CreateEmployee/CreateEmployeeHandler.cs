using Auth.Contracts.Features.RegisterUser;
using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using SharedWithUI.Auth.Dtos;
using System.Security.Claims;

namespace EmployeeModule.Employees.Features.Employees.CreateEmployee;

public record CreateEmployeeCommand(EmployeeDto Employee) : ICommand<CreateEmployeeResult>;
public record CreateEmployeeResult(EmployeeDto CreatedEmployee);
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{

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
        
        
        var employee = Employee.Create(
            Guid.NewGuid(),
            request.Employee.EmployeeNo,
            request.Employee.FirstName,
            request.Employee.MiddleName,
            request.Employee.LastName,
            request.Employee.Email,
            request.Employee.Phone,
            request.Employee.DateOfBirth,
            request.Employee.NationalId,
            request.Employee.HireDate,
            request.Employee.CompanyId!.Value,
            request.Employee.BranchId!.Value,
            request.Employee.AdministrationId!.Value,
            request.Employee.DepartmentId!.Value,
            request.Employee.PositionId!.Value,
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
