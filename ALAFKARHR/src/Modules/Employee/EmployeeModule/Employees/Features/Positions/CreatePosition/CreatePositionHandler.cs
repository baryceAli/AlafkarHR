using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Positions.CreatePosition;


public record CreatePositionCommand(PositionDto Position) : ICommand<CreatePositionResult>;
public record CreatePositionResult(PositionDto CreatedPosition);
public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(p=> p.Position.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(p => p.Position.BaseSalary).LessThanOrEqualTo(0).WithMessage("BaseSalary must be greator than 0");
    }
}
public class CreatePositionHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreatePositionCommand, CreatePositionResult>
{
    public async Task<CreatePositionResult> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var position = Position.Create(
                Guid.NewGuid(),
                request.Position.Title,
                request.Position.Code,
                request.Position.BaseSalary,
                request.Position.CompanyId,
                userId);

        await dbContext.Positions.AddAsync(position,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreatePositionResult(position.Adapt<PositionDto>());
    }
}
