namespace Contracts.Contracts.Features.Contracts;

public record GetContractsQuery(Guid? CompanyId, Guid? BranchId, string? PartyType, Guid? PartyId, ContractStatus? Status, string? Type, ContractRenewalPaymentStatus? PaymentStatus, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, string? SearchText)
    : IQuery<GetContractsResult>;

public record GetContractsResult(PaginatedResult<ContractDto> Contracts);

public record GetContractByIdQuery(Guid Id) : IQuery<GetContractByIdResult>;
public record GetContractByIdResult(ContractDto Contract);

public record CreateContractCommand(ContractDto Contract) : ICommand<CreateContractResult>;
public record CreateContractResult(Guid Id, string Number);

public record UpdateContractCommand(Guid Id, ContractDto Contract) : ICommand<UpdateContractResult>;
public record UpdateContractResult(bool IsSuccess);

public record DeleteContractCommand(Guid Id) : ICommand<DeleteContractResult>;
public record DeleteContractResult(bool IsSuccess);

public record ChangeContractStatusCommand(Guid Id, ContractStatus Status, string Action, string? Notes) : ICommand<ChangeContractStatusResult>;
public record ChangeContractStatusResult(bool IsSuccess);

public record ConfigureContractRenewalCommand(Guid Id, ContractRenewalSettingsDto Settings) : ICommand<ConfigureContractRenewalResult>;
public record ConfigureContractRenewalResult(bool IsSuccess);

public record ProcessContractRenewalCommand(Guid Id) : ICommand<ProcessContractRenewalResult>;
public record ProcessContractRenewalResult(ContractRenewalDto Renewal);

public record RecordContractRenewalPaymentCommand(Guid Id, Guid RenewalId, Guid? PaymentReferenceId, decimal PaidAmount) : ICommand<RecordContractRenewalPaymentResult>;
public record RecordContractRenewalPaymentResult(bool IsSuccess);
