using System.Security.Claims;
using FluentValidation;
using GeneralSettings.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.DeleteCurrency;

public record DeleteCurrencyCommand(Guid CompanyId, Guid CurrencyId) : ICommand<DeleteCurrencyResult>;
public record DeleteCurrencyResult(bool IsSuccess);

public class DeleteCurrencyCommandValidator : AbstractValidator<DeleteCurrencyCommand>
{
    public DeleteCurrencyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
    }
}

public class DeleteCurrencyHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteCurrencyCommand, DeleteCurrencyResult>
{
    public async Task<DeleteCurrencyResult> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        var currency = await dbContext.Currencies
            .FirstOrDefaultAsync(x => x.Id == request.CurrencyId && x.CompanyId == request.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Currency not found: {request.CurrencyId}");

        if (currency.IsDefault)
            throw new BadRequestException("Default currency cannot be deleted.");

        currency.Remove(userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteCurrencyResult(true);
    }
}
