using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializationById;


public record GetSpecializationByIdQuery(Guid Id) : IQuery<GetSpecializationByIdResult>;
public record GetSpecializationByIdResult(SpecializationDto Specialization);
public class GetSpecializationByIdHandler(EmployeeDbContext dbContext) : IQueryHandler<GetSpecializationByIdQuery, GetSpecializationByIdResult>
{
    public async Task<GetSpecializationByIdResult> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
    {
        var spec= await dbContext.Specializations.AsNoTracking().FirstOrDefaultAsync(s=> s.Id==request.Id, cancellationToken);
        if (spec is null)
            throw new NotFoundException($"Specialization not found: {request.Id}");

        return new GetSpecializationByIdResult(spec.Adapt<SpecializationDto>());
    }
}
