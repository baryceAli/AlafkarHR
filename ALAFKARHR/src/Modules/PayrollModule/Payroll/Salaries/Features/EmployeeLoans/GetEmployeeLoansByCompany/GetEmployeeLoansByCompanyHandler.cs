namespace Payroll.Salaries.Features.EmployeeLoans.GetEmployeeLoansByCompany;

public class GetEmployeeLoansByCompanyHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetEmployeeLoansByCompanyQuery, GetEmployeeLoansByCompanyResult>
{
    public async Task<GetEmployeeLoansByCompanyResult> Handle(GetEmployeeLoansByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmployeeLoans
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.Where(x =>
                (x.ReferenceNo != null && x.ReferenceNo.Contains(request.PaginationRequest.SearchText)) ||
                (x.Notes != null && x.Notes.Contains(request.PaginationRequest.SearchText)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var loans = await query
            .OrderByDescending(x => x.StartYear)
            .ThenByDescending(x => x.StartMonth)
            .ThenByDescending(x => x.CreatedAt)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new EmployeeLoanDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                EmployeeId = x.EmployeeId,
                Type = x.Type,
                Amount = x.Amount,
                InstallmentAmount = x.InstallmentAmount,
                DeductedAmount = x.DeductedAmount,
                RemainingAmount = x.Amount - x.DeductedAmount,
                StartMonth = x.StartMonth,
                StartYear = x.StartYear,
                Status = x.Status,
                ReferenceNo = x.ReferenceNo,
                Notes = x.Notes,
                ApprovedAt = x.ApprovedAt,
                ApprovedBy = x.ApprovedBy
            })
            .ToListAsync(cancellationToken);

        return new GetEmployeeLoansByCompanyResult(new PaginatedResult<EmployeeLoanDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            loans));
    }
}
