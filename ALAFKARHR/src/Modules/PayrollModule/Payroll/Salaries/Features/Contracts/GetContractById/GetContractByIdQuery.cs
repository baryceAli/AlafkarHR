using Payroll.Salaries.Features.Contracts.CreateContract;

namespace Payroll.Salaries.Features.Contracts.GetContractById;

public record GetContractByIdQuery(Guid Id) : IQuery<GetContractByIdResult>;

public record GetContractByIdResult(
    Guid Id,
    string Name,
    string NameEng,
    string? Description,
    Guid CompanyId,
    List<ContractItemDto> Items);
