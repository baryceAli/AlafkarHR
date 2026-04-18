using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Positions.GetPositionById;

public record GetPositionByIdQuery(Guid Id) : IQuery<GetPositionByIdResult>;
public record GetPositionByIdResult(PositionDto Position);
public class GetPositionByIdHandler(EmployeeDbContext dbContext) : IQueryHandler<GetPositionByIdQuery, GetPositionByIdResult>
{
    public async Task<GetPositionByIdResult> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var position=await dbContext.Positions.FirstOrDefaultAsync(p=>p.Id==request.Id,cancellationToken);
        if (position is null)
            throw new NotFoundException($"Position not found: {request.Id}");

        return new GetPositionByIdResult(position.Adapt<PositionDto>());
    }
}
