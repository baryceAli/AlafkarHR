namespace Organization.Organizations.Features.Administrations.GetAdministrationById;


public record GetAdministrationByIdQuery(Guid Id) : IQuery<GetAdministrationByIdResult>;
public record GetAdministrationByIdResult(AdministrationDto Administration);
public class GetAdministrationByIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetAdministrationByIdQuery, GetAdministrationByIdResult>
{
    public async Task<GetAdministrationByIdResult> Handle(GetAdministrationByIdQuery request, CancellationToken cancellationToken)
    {
        var administration = await dbContext.Administrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (administration is null)
            throw new NotFoundException($"Administration not found: {request.Id}");

        return new GetAdministrationByIdResult(administration.Adapt<AdministrationDto>());
    }
}
