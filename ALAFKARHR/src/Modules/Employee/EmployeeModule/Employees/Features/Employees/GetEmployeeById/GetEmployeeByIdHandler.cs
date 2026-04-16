using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeeById;


public record GetEmployeeByIdQuery(Guid Id) : IQuery<GetEmployeeByIdResult>;
public record GetEmployeeByIdResult(EmployeeDto Employee);
public class GetEmployeeByIdHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdResult>
{
    public async Task<GetEmployeeByIdResult> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee =await dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.Id}");

        return new GetEmployeeByIdResult(employee.Adapt<EmployeeDto>());
    }
}
