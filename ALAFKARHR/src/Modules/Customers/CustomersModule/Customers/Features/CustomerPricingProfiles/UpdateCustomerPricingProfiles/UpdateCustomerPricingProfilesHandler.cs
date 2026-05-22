namespace CustomersModule.Customers.Features.CustomerPricingProfiles.UpdateCustomerPricingProfiles;


public record UpdateCustomerPricingProfilesCommand(CustomerPricingProfileDto CustomerPricingProfile) : ICommand<UpdateCustomerPricingProfilesResult>;
public record UpdateCustomerPricingProfilesResult(bool IsSuccess);
public class UpdateCustomerPricingProfilesHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCustomerPricingProfilesCommand, UpdateCustomerPricingProfilesResult>
{
    public async Task<UpdateCustomerPricingProfilesResult> Handle(UpdateCustomerPricingProfilesCommand request, CancellationToken cancellationToken)
    {
        var customerPricingProfile = await dbContext.CustomerPricingProfiles.FirstOrDefaultAsync(c=>c.Id==request.CustomerPricingProfile.Id,cancellationToken);
        if (customerPricingProfile == null)
            throw new NotFoundException($"Customer pricing profile not found: {request.CustomerPricingProfile.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customerPricingProfile.Update(
            request.CustomerPricingProfile.PriceListId, 
            request.CustomerPricingProfile.DiscountPercentage,
            request.CustomerPricingProfile.AllowAdditionalDiscounts, 
            request.CustomerPricingProfile.EffectiveFrom,
            request.CustomerPricingProfile.EffectiveTo, 
            user);
    
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerPricingProfilesResult(true);
    }
}
