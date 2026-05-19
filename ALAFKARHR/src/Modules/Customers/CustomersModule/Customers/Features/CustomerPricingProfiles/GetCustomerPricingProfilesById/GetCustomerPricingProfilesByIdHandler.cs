namespace CustomersModule.Customers.Features.CustomerPricingProfiles.GetCustomerPricingProfilesById;

public record GetCustomerPricingProfilesByIdQuery(Guid Id) : IQuery<GetCustomerPricingProfilesByIdResult>;
public record GetCustomerPricingProfilesByIdResult(CustomerPricingProfileDto CustomerPricingProfile);
public class GetCustomerPricingProfilesByIdHandler(CustomerDbContext dbContext) :
    IQueryHandler<GetCustomerPricingProfilesByIdQuery, GetCustomerPricingProfilesByIdResult>
{
    public async Task<GetCustomerPricingProfilesByIdResult> Handle(GetCustomerPricingProfilesByIdQuery request, CancellationToken cancellationToken)
    {
        var customerPricingProfile = await dbContext.CustomerPricingProfiles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customerPricingProfile is null)
            throw new NotFoundException($"Customer pricing profile is not found: {request.Id}");

        return new GetCustomerPricingProfilesByIdResult(customerPricingProfile.Adapt<CustomerPricingProfileDto>());
    }
}
