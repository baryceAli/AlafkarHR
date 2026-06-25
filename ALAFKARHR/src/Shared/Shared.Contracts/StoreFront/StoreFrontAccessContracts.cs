using Shared.Contracts.CQRS;

namespace Shared.Contracts.StoreFront;

public record GetStoreFrontBranchScopeQuery(Guid StoreFrontId) : IQuery<GetStoreFrontBranchScopeResult>;

public record GetStoreFrontBranchScopeResult(Guid StoreFrontId, Guid CompanyId, Guid BranchId, Guid DefaultWarehouseId);

public record GetOpenPosCashierSessionQuery(Guid StoreFrontId) : IQuery<GetOpenPosCashierSessionResult>;

public record StoreFrontPosCashierSessionInfo(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid StoreFrontId,
    string CashierUserId,
    Guid? CashAccountId,
    decimal OpeningAmount,
    decimal ExpectedCashAmount,
    decimal CashSalesAmount,
    decimal CardSalesAmount,
    int PaymentCount,
    decimal? CountedCashAmount,
    decimal? VarianceAmount,
    int Status,
    DateTime OpenedAt,
    DateTime? ClosedAt);

public record GetOpenPosCashierSessionResult(StoreFrontPosCashierSessionInfo? Session);

public record EnsurePosCashierSessionForCheckoutQuery(Guid StoreFrontId, Guid CompanyId, Guid BranchId, int PaymentMethod)
    : IQuery<EnsurePosCashierSessionForCheckoutResult>;

public record EnsurePosCashierSessionForCheckoutResult(Guid? SessionId, Guid? CashAccountId);

public record RecordPosCashierSessionPaymentCommand(
    Guid StoreFrontId,
    Guid BranchId,
    Guid? SessionId,
    Guid PaymentId,
    int PaymentMethod,
    decimal Amount) : ICommand<RecordPosCashierSessionPaymentResult>;

public record RecordPosCashierSessionPaymentResult(bool IsSuccess);
