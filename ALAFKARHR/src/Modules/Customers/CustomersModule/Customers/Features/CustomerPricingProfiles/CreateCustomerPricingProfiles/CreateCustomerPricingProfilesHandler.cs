using CustomersModule.Customers.Models;
using FluentValidation;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.CreateCustomerPricingProfiles;


public record CreateCustomerPricingProfilesCommand(CustomerPricingProfileDto CustomerPricingProfile) : ICommand<CreateCustomerPricingProfilesResult>;
public record CreateCustomerPricingProfilesResult(Guid Id);

public class CreateCustomerPricingProfilesValidator : AbstractValidator<CreateCustomerPricingProfilesCommand>
{
    public CreateCustomerPricingProfilesValidator()
    {
        //RuleFor(x=> x.CustomerPricingProfile.)
    }
}
public class CreateCustomerPricingProfilesHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCustomerPricingProfilesCommand, CreateCustomerPricingProfilesResult>
{
    public async Task<CreateCustomerPricingProfilesResult> Handle(CreateCustomerPricingProfilesCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var customerPricingProfile = CustomerPricingProfile.Create(
            Guid.NewGuid(),
            command.CustomerPricingProfile.CustomerId,
            command.CustomerPricingProfile.PriceListId,
            command.CustomerPricingProfile.DiscountPercentage,
            command.CustomerPricingProfile.AllowAdditionalDiscounts,
            command.CustomerPricingProfile.EffectiveFrom,
            command.CustomerPricingProfile.EffectiveTo,
            command.CustomerPricingProfile.CompanyId.Value,
            user);

        await dbContext.CustomerPricingProfiles.AddAsync(customerPricingProfile,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerPricingProfilesResult(customerPricingProfile.Id);
    }
}
