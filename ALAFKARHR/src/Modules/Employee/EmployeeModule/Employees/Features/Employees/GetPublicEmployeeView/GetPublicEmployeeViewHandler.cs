using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Employees.GetPublicEmployeeView;

public record GetPublicEmployeeViewQuery(Guid Id) : IQuery<GetPublicEmployeeViewResult>;
public record GetPublicEmployeeViewResult(PublicEmployeeViewDto Employee);

public class GetPublicEmployeeViewHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetPublicEmployeeViewQuery, GetPublicEmployeeViewResult>
{
    public async Task<GetPublicEmployeeViewResult> Handle(GetPublicEmployeeViewQuery request, CancellationToken cancellationToken)
    {
        var employee = await (
                from e in dbContext.Employees.AsNoTracking()
                join p in dbContext.Positions.AsNoTracking() on e.PositionId equals p.Id into positions
                from p in positions.DefaultIfEmpty()
                where e.Id == request.Id && e.IsDeleted == false
                select new PublicEmployeeViewDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    FirstNameEng = e.FirstNameEng,
                    MiddleName = e.MiddleName,
                    MiddleNameEng = e.MiddleNameEng,
                    LastName = e.LastName,
                    LastNameEng = e.LastNameEng,
                    NationalId = e.NationalId,
                    Nationality = e.Nationality,
                    PositionId = e.PositionId,
                    PositionName = p == null ? null : p.Title,
                    PositionNameEng = p == null ? null : p.TitleEng,
                    BranchId = e.BranchId
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.Id}");

        return new GetPublicEmployeeViewResult(employee);
    }
}
