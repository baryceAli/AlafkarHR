using CustomersModule.Customers.Models;
using FluentValidation;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.CreateCustomerPricingProfiles;

public record CreateCustomerPricingProfilesCommand(CustomerPricingProfileDto CustomerPricingProfile) : ICommand<CreateCustomerPricingProfilesResult>;
public record CreateCustomerPricingProfilesResult(Guid Id);

public class CreateCustomerPricingProfilesValidator : AbstractValidator<CreateCustomerPricingProfilesCommand>
{
    public CreateCustomerPricingProfilesValidator(CustomerDbContext dbContext)
    {
        RuleFor(x => x.CustomerPricingProfile.CompanyId).NotNull().WithMessage("Company is required");
        RuleFor(x => x.CustomerPricingProfile.CustomerId).NotEmpty().WithMessage("Customer is required");
        RuleFor(x => x.CustomerPricingProfile.PriceListId).NotEmpty().WithMessage("Price list is required");
        RuleFor(x => x.CustomerPricingProfile.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomerPricingProfile.DiscountPercentage.HasValue);
        RuleFor(x => x.CustomerPricingProfile.EffectiveFrom).NotEmpty();
        RuleFor(x => x.CustomerPricingProfile)
            .Must(x => !x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= x.EffectiveFrom.Date)
            .WithMessage("Effective to must be greater than or equal to effective from");
        RuleFor(x => x.CustomerPricingProfile)
            .MustAsync(async (profile, cancellationToken) =>
            {
                if (!profile.CompanyId.HasValue)
                    return false;

                var profileFrom = profile.EffectiveFrom.Date;
                var profileTo = profile.EffectiveTo?.Date ?? DateTime.MaxValue.Date;

                return !await dbContext.CustomerPricingProfiles
                    .AsNoTracking()
                    .AnyAsync(existing =>
                        existing.CompanyId == profile.CompanyId.Value &&
                        existing.CustomerId == profile.CustomerId &&
                        existing.PriceListId == profile.PriceListId &&
                        existing.EffectiveFrom.Date <= profileTo &&
                        (!existing.EffectiveTo.HasValue || existing.EffectiveTo.Value.Date >= profileFrom),
                        cancellationToken);
            })
            .WithMessage("An overlapping pricing profile already exists for this customer and price list");
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

        await dbContext.CustomerPricingProfiles.AddAsync(customerPricingProfile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerPricingProfilesResult(customerPricingProfile.Id);
    }
}
