using Shared.Contracts.CQRS;
using Shared.Pagination;
using SharedWithUI.Catalog.Dtos;
//using SharedWithUI.Catalog.Dtos;

namespace Catalog.Contracts.Products.Features.GetProductByCompany;


public record GetProductByCompanyQuery(Guid companyId, PaginationRequest PaginationRequest)
    : IQuery<GetProductByCompanyResult>;

public record GetPricedProductByCompanyQuery(
    Guid companyId,
    Guid? CustomerId,
    Guid? PriceListId,
    PaginationRequest PaginationRequest) : IQuery<GetProductByCompanyResult>;

public record GetProductByCompanyResult(PaginatedResult<ProductDto> ProductList);
