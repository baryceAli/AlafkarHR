namespace Payroll.Salaries.Features.EmployeeContracts.GetEmployeeContractsByCompany;

public record GetEmployeeContractsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest)
    : IQuery<GetEmployeeContractsByCompanyResult>;

public record GetEmployeeContractsByCompanyResult(PaginatedResult<EmployeeContractDto> EmployeeContractList);
