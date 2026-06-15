using GeneralSettings.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Pagination;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.Currencies.GetCurrencies;


public record GetCurrenciesQuery(Guid companyId, PaginationRequest PaginationRequest) : IQuery<GetCurrenciesResult>;
public record GetCurrenciesResult(PaginatedResult<CurrencyDto> CurrencyList);
public class GetCurrenciesHandler(GeneralSettingsDbContext dbContext)
    : IQueryHandler<GetCurrenciesQuery, GetCurrenciesResult>
{
    public async Task<GetCurrenciesResult> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Currencies
            .AsNoTracking()
            .Where(c => c.CompanyId == request.companyId && !c.IsDeleted);

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

        var currencylist = await query.OrderBy(q => q.Code)
                                .ThenBy(q => q.Name)
                               .ThenBy(q => q.NameEng)
                               .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                               .Take(request.PaginationRequest.PageSize)
                               .ToListAsync(cancellationToken);

        var cdtoList = currencylist.Adapt<List<CurrencyDto>>();

        return new GetCurrenciesResult(
            new PaginatedResult<CurrencyDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                currencylist.Adapt<List<CurrencyDto>>()
                ));
    }
}
