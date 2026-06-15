using System.Security.Claims;
using FluentValidation;
using GeneralSettings.Data;
using GeneralSettings.GeneralSettings.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.Currencies.CreateCurrency;

public record CreateCurrencyCommand(Guid CompanyId, CurrencyDto Currency) : ICommand<CreateCurrencyResult>;
public record CreateCurrencyResult(CurrencyDto Currency);

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Currency.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Currency.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency.NameEng).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Currency.Value).GreaterThan(0);
    }
}

public class CreateCurrencyHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCurrencyCommand, CreateCurrencyResult>
{
    public async Task<CreateCurrencyResult> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        var code = request.Currency.Code.Trim().ToUpperInvariant();
        var exists = await dbContext.Currencies.AnyAsync(
            x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.Code == code,
            cancellationToken);

        if (exists)
            throw new BadRequestException("Currency code already exists for this company.");

        if (request.Currency.IsDefault)
        {
            var defaults = await dbContext.Currencies
                .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var defaultCurrency in defaults)
            {
                defaultCurrency.SetDefault(false, userId);
            }
        }

        var currency = Currency.Create(
            Guid.NewGuid(),
            code,
            request.Currency.Name.Trim(),
            request.Currency.NameEng.Trim(),
            request.Currency.Value,
            request.Currency.Symbol.Trim(),
            request.Currency.IsDefault,
            request.CompanyId,
            userId);

        dbContext.Currencies.Add(currency);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCurrencyResult(currency.Adapt<CurrencyDto>());
    }
}
