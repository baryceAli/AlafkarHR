using CustomersModule.Customers.Models;
using FluentValidation;

namespace CustomersModule.Customers.Features.CustomerGroups.CreateCustomerGroup;

public record CreateCustomerGroupCommand(CustomerGroupDto CustomerGroup) : ICommand<CreateCustomerGroupResult>;
public record CreateCustomerGroupResult(Guid Id);

public class CreateCustomerGroupCommandValidator : AbstractValidator<CreateCustomerGroupCommand>
{
    public CreateCustomerGroupCommandValidator()
    {
        RuleFor(x => x.CustomerGroup.Name).NotEmpty().MaximumLength(150).WithMessage("Name is required");
        RuleFor(x => x.CustomerGroup.NameEng).NotEmpty().MaximumLength(150).WithMessage("English name is required");
        RuleFor(x => x.CustomerGroup.Description).MaximumLength(1000);
        RuleFor(x => x.CustomerGroup.DefaultDiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomerGroup.DefaultDiscountPercentage.HasValue);
        RuleFor(x => x.CustomerGroup.CompanyId).NotNull().WithMessage("Company is required");
    }
}

public class CreateCustomerGroupHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCustomerGroupCommand, CreateCustomerGroupResult>
{
    public async Task<CreateCustomerGroupResult> Handle(CreateCustomerGroupCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        var customer = CustomerGroup.Create(
                Guid.NewGuid(),
                command.CustomerGroup.Name,
                command.CustomerGroup.NameEng,
                command.CustomerGroup.Description,
                command.CustomerGroup.DefaultDiscountPercentage,
                command.CustomerGroup.DefaultPriceListId,
                command.CustomerGroup.CompanyId.Value,
                user);

        await dbContext.CustomerGroups.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerGroupResult(customer.Id);
    }
}
