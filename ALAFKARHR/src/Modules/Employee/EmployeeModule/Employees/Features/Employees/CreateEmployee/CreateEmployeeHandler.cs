using EmployeeModule.Employees.Dtos;
using EmployeeModule.Employees.Models;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.CQRS;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Employees.CreateEmployee;

public record CreateEmployeeCommand(EmployeeDto Employee) : ICommand<CreateEmployeeResult>;
public record CreateEmployeeResult(EmployeeDto CreatedEmployee);
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{

}
public class CreateEmployeeHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
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
            request.Employee.CompanyId,
            request.Employee.BranchId,
            request.Employee.AdministrationId,
            request.Employee.DepartmentId,
            request.Employee.PositionId,
            userId);
        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateEmployeeResult(employee.Adapt<EmployeeDto>());
    }
}
