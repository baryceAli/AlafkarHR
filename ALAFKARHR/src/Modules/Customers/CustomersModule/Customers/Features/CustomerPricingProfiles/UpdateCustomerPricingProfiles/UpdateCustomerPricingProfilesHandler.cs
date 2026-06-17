using FluentValidation;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.UpdateCustomerPricingProfiles;

public record UpdateCustomerPricingProfilesCommand(CustomerPricingProfileDto CustomerPricingProfile) : ICommand<UpdateCustomerPricingProfilesResult>;
public record UpdateCustomerPricingProfilesResult(bool IsSuccess);

public class UpdateCustomerPricingProfilesValidator : AbstractValidator<UpdateCustomerPricingProfilesCommand>
{
    public UpdateCustomerPricingProfilesValidator(CustomerDbContext dbContext)
    {
        RuleFor(x => x.CustomerPricingProfile.Id).NotEmpty();
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
                var current = await dbContext.CustomerPricingProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == profile.Id, cancellationToken);

                if (current is null)
                    return true;

                var profileFrom = profile.EffectiveFrom.Date;
                var profileTo = profile.EffectiveTo?.Date ?? DateTime.MaxValue.Date;

                return !await dbContext.CustomerPricingProfiles
                    .AsNoTracking()
                    .AnyAsync(existing =>
                        existing.Id != profile.Id &&
                        existing.CompanyId == current.CompanyId &&
                        existing.CustomerId == current.CustomerId &&
                        existing.PriceListId == profile.PriceListId &&
                        existing.EffectiveFrom.Date <= profileTo &&
                        (!existing.EffectiveTo.HasValue || existing.EffectiveTo.Value.Date >= profileFrom),
                        cancellationToken);
            })
            .WithMessage("An overlapping pricing profile already exists for this customer and price list");
    }
}

public class UpdateCustomerPricingProfilesHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCustomerPricingProfilesCommand, UpdateCustomerPricingProfilesResult>
{
    public async Task<UpdateCustomerPricingProfilesResult> Handle(UpdateCustomerPricingProfilesCommand request, CancellationToken cancellationToken)
    {
        var customerPricingProfile = await dbContext.CustomerPricingProfiles.FirstOrDefaultAsync(c => c.Id == request.CustomerPricingProfile.Id, cancellationToken);
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
