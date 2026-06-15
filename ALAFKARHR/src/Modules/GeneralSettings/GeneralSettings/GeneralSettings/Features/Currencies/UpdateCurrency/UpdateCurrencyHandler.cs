using System.Security.Claims;
using FluentValidation;
using GeneralSettings.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.Currencies.UpdateCurrency;

public record UpdateCurrencyCommand(Guid CompanyId, Guid CurrencyId, CurrencyDto Currency) : ICommand<UpdateCurrencyResult>;
public record UpdateCurrencyResult(CurrencyDto Currency);

public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.Currency.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Currency.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency.NameEng).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Currency.Value).GreaterThan(0);
    }
}

public class UpdateCurrencyHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCurrencyCommand, UpdateCurrencyResult>
{
    public async Task<UpdateCurrencyResult> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        var currency = await dbContext.Currencies
            .FirstOrDefaultAsync(x => x.Id == request.CurrencyId && x.CompanyId == request.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Currency not found: {request.CurrencyId}");

        var code = request.Currency.Code.Trim().ToUpperInvariant();
        var duplicate = await dbContext.Currencies.AnyAsync(
            x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.Id != request.CurrencyId && x.Code == code,
            cancellationToken);

        if (duplicate)
            throw new BadRequestException("Currency code already exists for this company.");

        if (request.Currency.IsDefault)
        {
            var defaults = await dbContext.Currencies
                .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.Id != request.CurrencyId && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var defaultCurrency in defaults)
            {
                defaultCurrency.SetDefault(false, userId);
            }
        }
        else if (currency.IsDefault)
        {
            var hasOtherDefault = await dbContext.Currencies.AnyAsync(
                x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.Id != request.CurrencyId && x.IsDefault,
                cancellationToken);

            if (!hasOtherDefault)
                throw new BadRequestException("At least one default currency is required.");
        }

        currency.Update(
            code,
            request.Currency.Name.Trim(),
            request.Currency.NameEng.Trim(),
            request.Currency.Value,
            request.Currency.Symbol.Trim(),
            request.Currency.IsDefault,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCurrencyResult(currency.Adapt<CurrencyDto>());
    }
}
