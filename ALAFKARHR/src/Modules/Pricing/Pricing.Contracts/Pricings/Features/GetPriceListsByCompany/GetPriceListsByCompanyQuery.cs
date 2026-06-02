
using Shared.Contracts.CQRS;
using Shared.Pagination;
using SharedWithUI.Pricing.Dtos;

public record GetPriceListsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetPriceListsByCompanyResult>;
public record GetPriceListsByCompanyResult(PaginatedResult<PriceListDto> PriceList);
