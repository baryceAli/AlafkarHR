using FluentValidation;

namespace CustomersModule.Customers.Features.CustomerGroups.UpdateCustomerGroup;

public record UpdateCustomerGroupCommand(CustomerGroupDto CustomerGroup) : ICommand<UpdateCustomerGroupResult>;
public record UpdateCustomerGroupResult(bool IsSuccess);

public class UpdateCustomerGroupCommandValidator : AbstractValidator<UpdateCustomerGroupCommand>
{
    public UpdateCustomerGroupCommandValidator()
    {
        RuleFor(x => x.CustomerGroup.Id).NotEmpty();
        RuleFor(x => x.CustomerGroup.Name).NotEmpty().MaximumLength(150).WithMessage("Name is required");
        RuleFor(x => x.CustomerGroup.NameEng).NotEmpty().MaximumLength(150).WithMessage("English name is required");
        RuleFor(x => x.CustomerGroup.Description).MaximumLength(1000);
        RuleFor(x => x.CustomerGroup.DefaultDiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomerGroup.DefaultDiscountPercentage.HasValue);
    }
}

public class UpdateCustomerGroupHanlder(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCustomerGroupCommand, UpdateCustomerGroupResult>
{
    public async Task<UpdateCustomerGroupResult> Handle(UpdateCustomerGroupCommand request, CancellationToken cancellationToken)
    {
        var customerGroup = await dbContext.CustomerGroups.FirstOrDefaultAsync(c => c.Id == request.CustomerGroup.Id, cancellationToken);

        if (customerGroup is null)
            throw new NotFoundException($"Customer group not found: {request.CustomerGroup.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customerGroup.Update(
            request.CustomerGroup.Name,
            request.CustomerGroup.NameEng,
            request.CustomerGroup.Description,
            request.CustomerGroup.DefaultDiscountPercentage,
            request.CustomerGroup.DefaultPriceListId,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerGroupResult(true);
    }
}
