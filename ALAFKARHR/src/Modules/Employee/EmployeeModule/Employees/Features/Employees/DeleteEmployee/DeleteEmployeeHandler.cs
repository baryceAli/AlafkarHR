using Shared.Contracts.CQRS;

namespace EmployeeModule.Employees.Features.Employees.DeleteEmployee;


public record DeleteEmployeeCommand(Guid EmployeeId) : ICommand<DeleteEmployeeResult>;
public record DeleteEmployeeResult(bool IsSuccess);
public class DeleteEmployeeHandler(EmployeeDbContext dbContext) : ICommandHandler<DeleteEmployeeCommand, DeleteEmployeeResult>
{
    public async Task<DeleteEmployeeResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
        if (employee == null)
            throw new Exception("Employee not found");
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteEmployeeResult(true);
    }
}


