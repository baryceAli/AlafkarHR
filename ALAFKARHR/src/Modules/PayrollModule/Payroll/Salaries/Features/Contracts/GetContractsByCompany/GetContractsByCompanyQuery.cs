namespace Payroll.Salaries.Features.Contracts.GetContractsByCompany;

public record GetContractsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest)
    : IQuery<GetContractsByCompanyResult>;

public record GetContractsByCompanyResult(PaginatedResult<ContractDto> ContractList);
