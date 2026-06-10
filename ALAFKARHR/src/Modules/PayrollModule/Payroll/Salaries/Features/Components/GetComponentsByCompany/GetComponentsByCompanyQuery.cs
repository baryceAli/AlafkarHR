namespace Payroll.Salaries.Features.Components.GetComponentsByCompany;

public record GetComponentsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest)
    : IQuery<GetComponentsByCompanyResult>;

public record GetComponentsByCompanyResult(PaginatedResult<ComponentDto> ComponentList);
