namespace Organization.Organizations.Features.Administrations.GetAdministrations;


public record GetAdministrationsQuery(PaginationRequest PaginationRequest):IQuery<GetAdministrationsResult>;
public record GetAdministrationsResult(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetAdministrationsQuery, GetAdministrationsResult>
{
    public async Task<GetAdministrationsResult> Handle(GetAdministrationsQuery request, CancellationToken cancellationToken)
    {
        var count = await dbContext.Administrations.LongCountAsync(cancellationToken);

        var administrations = await dbContext.Administrations
            .AsNoTracking()
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetAdministrationsResult(
            new PaginatedResult<AdministrationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                administrations.Adapt<List<AdministrationDto>>()));
    }
}
