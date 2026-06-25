using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeeById;


public record GetEmployeeByIdQuery(Guid Id) : IQuery<GetEmployeeByIdResult>;
public record GetEmployeeByIdResult(EmployeeDto Employee);
public class GetEmployeeByIdHandler(EmployeeDbContext dbContext, ISender sender)
    : IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdResult>
{
    public async Task<GetEmployeeByIdResult> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee =await dbContext.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                            e => e.Id == request.Id && 
                            e.IsDeleted==false ,
                            cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.Id}");
        await EmployeeModule.Employees.Features.Employees.EmployeeBranchScope.EnsureCanReadAsync(sender, employee.CompanyId, employee.BranchId, cancellationToken);

        //var employeeDto= await (from pos in dbContext.Positions
        //                        join emp in dbContext.Employees on pos.Id equals emp.PositionId
        //                        where emp.Id == request.Id
        //                        )

        return new GetEmployeeByIdResult(employee.Adapt<EmployeeDto>());
    }
}
