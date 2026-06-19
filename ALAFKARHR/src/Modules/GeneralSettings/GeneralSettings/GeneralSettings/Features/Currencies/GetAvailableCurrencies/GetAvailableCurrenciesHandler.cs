using GeneralSettings.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Pagination;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.Currencies.GetAvailableCurrencies;

public record GetAvailableCurrenciesQuery(PaginationRequest PaginationRequest) : IQuery<GetAvailableCurrenciesResult>;
public record GetAvailableCurrenciesResult(PaginatedResult<CurrencyDto> CurrencyList);

public class GetAvailableCurrenciesHandler(GeneralSettingsDbContext dbContext)
    : IQueryHandler<GetAvailableCurrenciesQuery, GetAvailableCurrenciesResult>
{
    public async Task<GetAvailableCurrenciesResult> Handle(GetAvailableCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Currencies
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var search = searchText.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search) ||
                                     c.NameEng.ToLower().Contains(search) ||
                                     c.Code.ToLower().Contains(search) ||
                                     c.Symbol.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var currencies = await query
            .OrderByDescending(c => c.IsDefault)
            .ThenBy(c => c.Code)
            .ThenBy(c => c.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetAvailableCurrenciesResult(
            new PaginatedResult<CurrencyDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                currencies.Adapt<List<CurrencyDto>>()));
    }
}
