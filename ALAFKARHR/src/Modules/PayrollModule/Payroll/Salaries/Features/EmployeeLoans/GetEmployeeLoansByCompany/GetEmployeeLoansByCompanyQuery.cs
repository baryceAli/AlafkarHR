namespace Payroll.Salaries.Features.EmployeeLoans.GetEmployeeLoansByCompany;

public record GetEmployeeLoansByCompanyQuery(
    Guid CompanyId,
    Guid? EmployeeId,
    EmployeeLoanStatus? Status,
    PaginationRequest PaginationRequest) : IQuery<GetEmployeeLoansByCompanyResult>;

public record GetEmployeeLoansByCompanyResult(PaginatedResult<EmployeeLoanDto> EmployeeLoanList);
