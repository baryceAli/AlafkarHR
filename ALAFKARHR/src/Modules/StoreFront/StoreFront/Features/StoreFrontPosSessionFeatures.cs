namespace StoreFront.Features;

public record OpenPosCashierSessionCommand(OpenPosCashierSessionDto Session) : ICommand<OpenPosCashierSessionResult>;
public record OpenPosCashierSessionResult(PosCashierSessionDto Session);
public record ClosePosCashierSessionCommand(Guid SessionId, ClosePosCashierSessionDto Close) : ICommand<ClosePosCashierSessionResult>;
public record ClosePosCashierSessionResult(PosCashierSessionDto Session);
public record GetStoreFrontCashAccountsQuery(Guid StoreFrontId) : IQuery<GetStoreFrontCashAccountsResult>;
public record GetStoreFrontCashAccountsResult(List<CashAccountDto> CashAccounts);
public record UpsertStoreFrontCashAccountCommand(Guid StoreFrontId, CashAccountDto CashAccount) : ICommand<UpsertStoreFrontCashAccountResult>;
public record UpsertStoreFrontCashAccountResult(Guid Id);
public record GetOpenPosCashierSessionsQuery(Guid StoreFrontId) : IQuery<GetOpenPosCashierSessionsResult>;
public record GetOpenPosCashierSessionsResult(List<PosCashierSessionDto> Sessions);
public record GetPosCashierSessionsQuery(Guid StoreFrontId, DateTime? FromDate, DateTime? ToDate, bool OwnOnly = false) : IQuery<GetPosCashierSessionsResult>;
public record GetPosCashierSessionsResult(List<PosCashierSessionDto> Sessions);
public record GetPosCashierSessionSummaryQuery(Guid StoreFrontId, DateTime? FromDate, DateTime? ToDate, bool OwnOnly = false) : IQuery<GetPosCashierSessionSummaryResult>;
public record GetPosCashierSessionSummaryResult(PosCashierSessionSummaryDto Summary);

public record OpenPosCashierSessionRequest(OpenPosCashierSessionDto Session);
public record ClosePosCashierSessionRequest(ClosePosCashierSessionDto Close);
public record UpsertStoreFrontCashAccountRequest(CashAccountDto CashAccount);

public class StoreFrontPosSessionHandler(StoreFrontDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<OpenPosCashierSessionCommand, OpenPosCashierSessionResult>,
      ICommandHandler<ClosePosCashierSessionCommand, ClosePosCashierSessionResult>,
      ICommandHandler<UpsertStoreFrontCashAccountCommand, UpsertStoreFrontCashAccountResult>,
      ICommandHandler<RecordPosCashierSessionPaymentCommand, RecordPosCashierSessionPaymentResult>,
      IQueryHandler<GetStoreFrontCashAccountsQuery, GetStoreFrontCashAccountsResult>,
      IQueryHandler<GetOpenPosCashierSessionQuery, GetOpenPosCashierSessionResult>,
      IQueryHandler<GetOpenPosCashierSessionsQuery, GetOpenPosCashierSessionsResult>,
      IQueryHandler<EnsurePosCashierSessionForCheckoutQuery, EnsurePosCashierSessionForCheckoutResult>,
      IQueryHandler<GetPosCashierSessionsQuery, GetPosCashierSessionsResult>,
      IQueryHandler<GetPosCashierSessionSummaryQuery, GetPosCashierSessionSummaryResult>
{
    public async Task<GetStoreFrontCashAccountsResult> Handle(GetStoreFrontCashAccountsQuery request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanReadCashAccountsAsync(scope.CompanyId, scope.BranchId, cancellationToken);
        var result = await sender.Send(new GetAccountingCashAccountsQuery(scope.CompanyId, scope.BranchId), cancellationToken);
        return new GetStoreFrontCashAccountsResult(result.CashAccounts);
    }

    public async Task<UpsertStoreFrontCashAccountResult> Handle(UpsertStoreFrontCashAccountCommand request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanMutateSessionAsync(scope.CompanyId, scope.BranchId, PermissionList.StoreFrontPosPermissions.ManageCashAccounts, cancellationToken);

        var dto = request.CashAccount;
        dto.CompanyId = scope.CompanyId;
        dto.BranchId = scope.BranchId;
        if (dto.Id != Guid.Empty)
        {
            var existing = await sender.Send(new GetAccountingCashAccountsQuery(scope.CompanyId, scope.BranchId), cancellationToken);
            if (!existing.CashAccounts.Any(x => x.Id == dto.Id))
                throw new ForbiddenException("Cash account does not belong to this StoreFront branch.");
        }

        var result = await sender.Send(new UpsertAccountingCashAccountCommand(dto), cancellationToken);
        return new UpsertStoreFrontCashAccountResult(result.Id);
    }

    public async Task<OpenPosCashierSessionResult> Handle(OpenPosCashierSessionCommand request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.Session.StoreFrontId), cancellationToken);
        await EnsureCanMutateSessionAsync(scope.CompanyId, scope.BranchId, PermissionList.StoreFrontPosPermissions.OpenSession, cancellationToken);
        if (request.Session.CashAccountId.HasValue)
            await EnsureCashAccountAsync(scope.CompanyId, scope.BranchId, request.Session.CashAccountId.Value, cancellationToken);

        var userId = CurrentUserId();
        var existing = await dbContext.PosCashierSessions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StoreFrontId == scope.StoreFrontId
                && x.CashierUserId == userId
                && x.Status == PosCashierSessionStatus.Open, cancellationToken);
        if (existing is not null)
            return new OpenPosCashierSessionResult(existing.ToDto());

        var session = PosCashierSession.Open(
            scope.CompanyId,
            scope.BranchId,
            scope.StoreFrontId,
            userId,
            request.Session.CashAccountId,
            request.Session.OpeningAmount);

        await dbContext.PosCashierSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new OpenPosCashierSessionResult(session.ToDto());
    }

    public async Task<ClosePosCashierSessionResult> Handle(ClosePosCashierSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.PosCashierSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException($"Cashier session not found: {request.SessionId}");
        await EnsureCanMutateSessionAsync(session.CompanyId, session.BranchId, PermissionList.StoreFrontPosPermissions.CloseSession, cancellationToken);

        if (request.Close.HandoverToSessionId.HasValue || request.Close.HandoverToCashAccountId.HasValue)
            await sender.Send(new EnsureCurrentUserBranchPermissionQuery(session.CompanyId, session.BranchId, PermissionList.StoreFrontPosPermissions.HandoverCash), cancellationToken);

        if (request.Close.HandoverToSessionId.HasValue)
        {
            var receiver = await dbContext.PosCashierSessions
                .FirstOrDefaultAsync(x => x.Id == request.Close.HandoverToSessionId.Value
                    && x.CompanyId == session.CompanyId
                    && x.BranchId == session.BranchId
                    && x.StoreFrontId == session.StoreFrontId
                    && x.Status == PosCashierSessionStatus.Open, cancellationToken);
            if (receiver is null)
                throw new BadRequestException("Handover target session must be an open session in the same StoreFront branch.");
            receiver.ReceiveHandover(request.Close.CountedCashAmount, CurrentUserId());
        }

        if (request.Close.HandoverToCashAccountId.HasValue)
        {
            await EnsureCashAccountAsync(session.CompanyId, session.BranchId, request.Close.HandoverToCashAccountId.Value, cancellationToken);
        }

        session.Close(request.Close.CountedCashAmount, CurrentUserId());

        if (request.Close.HandoverToSessionId.HasValue || request.Close.HandoverToCashAccountId.HasValue)
        {
            await dbContext.PosCashierSessionTransfers.AddAsync(
                PosCashierSessionTransfer.Create(
                    session.CompanyId,
                    session.BranchId,
                    session.StoreFrontId,
                    session.Id,
                    request.Close.HandoverToSessionId,
                    request.Close.HandoverToCashAccountId,
                    request.Close.CountedCashAmount,
                    CurrentUserId()),
                cancellationToken);
        }

        if (request.Close.HandoverToCashAccountId.HasValue
            && session.CashAccountId.HasValue
            && session.CashAccountId.Value != request.Close.HandoverToCashAccountId.Value
            && request.Close.CountedCashAmount > 0)
        {
            var source = await EnsureCashAccountAsync(session.CompanyId, session.BranchId, session.CashAccountId.Value, cancellationToken);
            var target = await EnsureCashAccountAsync(session.CompanyId, session.BranchId, request.Close.HandoverToCashAccountId.Value, cancellationToken);
            await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
            {
                CompanyId = session.CompanyId,
                BranchId = session.BranchId,
                EntryDate = DateTime.UtcNow,
                SourceModule = "StoreFrontPOS",
                SourceDocumentId = session.Id,
                SourceDocumentNumber = $"POS-CLOSE-{session.Id:N}",
                Memo = "StoreFront POS cash handover",
                Lines =
                [
                    new() { AccountId = target.LedgerAccountId, Debit = request.Close.CountedCashAmount, Description = "Cash handover in" },
                    new() { AccountId = source.LedgerAccountId, Credit = request.Close.CountedCashAmount, Description = "Cash handover out" }
                ]
            }), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ClosePosCashierSessionResult(session.ToDto());
    }

    public async Task<GetOpenPosCashierSessionResult> Handle(GetOpenPosCashierSessionQuery request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanReadSessionAsync(scope.CompanyId, scope.BranchId, PermissionList.StoreFrontPosPermissions.ViewOwnSummary, cancellationToken);
        var userId = CurrentUserId();
        var session = await dbContext.PosCashierSessions.AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId && x.CashierUserId == userId && x.Status == PosCashierSessionStatus.Open)
            .OrderByDescending(x => x.OpenedAt)
            .FirstOrDefaultAsync(cancellationToken);
        return new GetOpenPosCashierSessionResult(session is null ? null : ToContractInfo(session));
    }

    public async Task<GetOpenPosCashierSessionsResult> Handle(GetOpenPosCashierSessionsQuery request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanReadSessionAsync(scope.CompanyId, scope.BranchId, PermissionList.StoreFrontPosPermissions.HandoverCash, cancellationToken);
        var userId = CurrentUserId();
        var sessions = await dbContext.PosCashierSessions.AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId
                && x.BranchId == scope.BranchId
                && x.Status == PosCashierSessionStatus.Open
                && x.CashierUserId != userId)
            .OrderBy(x => x.OpenedAt)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new GetOpenPosCashierSessionsResult(sessions);
    }

    public async Task<EnsurePosCashierSessionForCheckoutResult> Handle(EnsurePosCashierSessionForCheckoutQuery request, CancellationToken cancellationToken)
    {
        await EnsureCanReadSessionAsync(request.CompanyId, request.BranchId, PermissionList.StoreFrontPosPermissions.Checkout, cancellationToken);
        var userId = CurrentUserId();
        var session = await dbContext.PosCashierSessions.AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId && x.BranchId == request.BranchId && x.CashierUserId == userId && x.Status == PosCashierSessionStatus.Open)
            .OrderByDescending(x => x.OpenedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var paymentMethod = (PaymentMethodType)request.PaymentMethod;
        if (paymentMethod == PaymentMethodType.Cash && session is null)
            throw new BadRequestException("Open a cashier session before accepting cash checkout.");

        return new EnsurePosCashierSessionForCheckoutResult(session?.Id, session?.CashAccountId);
    }

    public async Task<RecordPosCashierSessionPaymentResult> Handle(RecordPosCashierSessionPaymentCommand request, CancellationToken cancellationToken)
    {
        if (!request.SessionId.HasValue)
        {
            if ((PaymentMethodType)request.PaymentMethod == PaymentMethodType.Cash)
                throw new BadRequestException("Cash checkout requires an open cashier session.");
            return new RecordPosCashierSessionPaymentResult(true);
        }

        var session = await dbContext.PosCashierSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId.Value, cancellationToken)
            ?? throw new NotFoundException($"Cashier session not found: {request.SessionId}");
        if (session.StoreFrontId != request.StoreFrontId || session.BranchId != request.BranchId || session.Status != PosCashierSessionStatus.Open)
            throw new BadRequestException("Payment session does not match the StoreFront branch.");

        session.RecordPayment(request.PaymentId, (PaymentMethodType)request.PaymentMethod, request.Amount, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RecordPosCashierSessionPaymentResult(true);
    }

    public async Task<GetPosCashierSessionSummaryResult> Handle(GetPosCashierSessionSummaryQuery request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanReadSessionAsync(scope.CompanyId, scope.BranchId, request.OwnOnly ? PermissionList.StoreFrontPosPermissions.ViewOwnSummary : PermissionList.StoreFrontPosPermissions.ViewBranchSummaries, cancellationToken);

        var fromDate = (request.FromDate ?? DateTime.UtcNow.Date).Date;
        var toDate = (request.ToDate ?? DateTime.UtcNow.Date).Date.AddDays(1).AddTicks(-1);
        var query = dbContext.PosCashierSessions.AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId && x.OpenedAt >= fromDate && x.OpenedAt <= toDate);
        if (request.OwnOnly)
        {
            var userId = CurrentUserId();
            query = query.Where(x => x.CashierUserId == userId);
        }
        var sessions = await query.ToListAsync(cancellationToken);

        return new GetPosCashierSessionSummaryResult(new PosCashierSessionSummaryDto
        {
            StoreFrontId = scope.StoreFrontId,
            BranchId = scope.BranchId,
            FromDate = fromDate,
            ToDate = toDate.Date,
            CashSalesAmount = sessions.Sum(x => x.CashSalesAmount),
            CardSalesAmount = sessions.Sum(x => x.CardSalesAmount),
            ExpectedCashAmount = sessions.Sum(x => x.ExpectedCashAmount),
            CountedCashAmount = sessions.Sum(x => x.CountedCashAmount ?? 0m),
            VarianceAmount = sessions.Sum(x => x.VarianceAmount ?? 0m),
            SessionCount = sessions.Count,
            PaymentCount = sessions.Sum(x => x.PaymentCount)
        });
    }

    public async Task<GetPosCashierSessionsResult> Handle(GetPosCashierSessionsQuery request, CancellationToken cancellationToken)
    {
        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(request.StoreFrontId), cancellationToken);
        await EnsureCanReadSessionAsync(scope.CompanyId, scope.BranchId, request.OwnOnly ? PermissionList.StoreFrontPosPermissions.ViewOwnSummary : PermissionList.StoreFrontPosPermissions.ViewBranchSummaries, cancellationToken);

        var fromDate = (request.FromDate ?? DateTime.UtcNow.Date).Date;
        var toDate = (request.ToDate ?? DateTime.UtcNow.Date).Date.AddDays(1).AddTicks(-1);
        var query = dbContext.PosCashierSessions.AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId && x.OpenedAt >= fromDate && x.OpenedAt <= toDate);
        if (request.OwnOnly)
        {
            var userId = CurrentUserId();
            query = query.Where(x => x.CashierUserId == userId);
        }
        var sessions = await query.OrderByDescending(x => x.OpenedAt).ToListAsync(cancellationToken);
        var sessionIds = sessions.Select(x => x.Id).ToList();
        var transfers = await dbContext.PosCashierSessionTransfers.AsNoTracking()
            .Where(x => sessionIds.Contains(x.FromSessionId))
            .ToDictionaryAsync(x => x.FromSessionId, cancellationToken);

        return new GetPosCashierSessionsResult(sessions.Select(session =>
        {
            transfers.TryGetValue(session.Id, out var transfer);
            return ToDto(session, transfer);
        }).ToList());
    }

    private async Task EnsureCanReadSessionAsync(Guid companyId, Guid branchId, string permission, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(access, branchId))
            throw new ForbiddenException("You do not have permission to view this StoreFront branch.");
        await sender.Send(new EnsureCurrentUserBranchPermissionQuery(companyId, branchId, permission), cancellationToken);
    }

    private async Task EnsureCanReadCashAccountsAsync(Guid companyId, Guid branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(access, branchId))
            throw new ForbiddenException("You do not have permission to view this StoreFront branch.");

        try
        {
            await sender.Send(new EnsureCurrentUserBranchPermissionQuery(companyId, branchId, PermissionList.StoreFrontPosPermissions.OpenSession), cancellationToken);
        }
        catch (ForbiddenException)
        {
            await sender.Send(new EnsureCurrentUserBranchPermissionQuery(companyId, branchId, PermissionList.StoreFrontPosPermissions.ManageCashAccounts), cancellationToken);
        }
    }

    private async Task EnsureCanMutateSessionAsync(Guid companyId, Guid branchId, string permission, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(access, branchId))
            throw new ForbiddenException("You do not have permission to change this StoreFront branch.");
        await sender.Send(new EnsureCurrentUserBranchPermissionQuery(companyId, branchId, permission), cancellationToken);
    }

    private string CurrentUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    private async Task<GetAccountingCashAccountScopeResult> EnsureCashAccountAsync(Guid companyId, Guid branchId, Guid cashAccountId, CancellationToken cancellationToken) =>
        await sender.Send(new GetAccountingCashAccountScopeQuery(companyId, branchId, cashAccountId), cancellationToken);

    private static StoreFrontPosCashierSessionInfo ToContractInfo(PosCashierSession session) => new(
        session.Id,
        session.CompanyId,
        session.BranchId,
        session.StoreFrontId,
        session.CashierUserId,
        session.CashAccountId,
        session.OpeningAmount,
        session.ExpectedCashAmount,
        session.CashSalesAmount,
        session.CardSalesAmount,
        session.PaymentCount,
        session.CountedCashAmount,
        session.VarianceAmount,
        (int)session.Status,
        session.OpenedAt,
        session.ClosedAt);

    private static PosCashierSessionDto ToDto(PosCashierSession session, PosCashierSessionTransfer? transfer)
    {
        var dto = session.ToDto();
        if (transfer is not null)
        {
            dto.HandoverToSessionId = transfer.ToSessionId;
            dto.HandoverToCashAccountId = transfer.ToCashAccountId;
            dto.HandoverAmount = transfer.Amount;
        }
        return dto;
    }
}

public class StoreFrontPosSessionEndpoints : ICarterModule
{
    private const string Route = "/api/v1/store-front";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Route}/stores/{{storeFrontId:guid}}/cash-accounts", async (Guid storeFrontId, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontCashAccountsQuery(storeFrontId));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPost($"{Route}/stores/{{storeFrontId:guid}}/cash-accounts", async (Guid storeFrontId, UpsertStoreFrontCashAccountRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpsertStoreFrontCashAccountCommand(storeFrontId, request.CashAccount));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPut($"{Route}/stores/{{storeFrontId:guid}}/cash-accounts/{{cashAccountId:guid}}", async (Guid storeFrontId, Guid cashAccountId, UpsertStoreFrontCashAccountRequest request, ISender sender) =>
        {
            request.CashAccount.Id = cashAccountId;
            var result = await sender.Send(new UpsertStoreFrontCashAccountCommand(storeFrontId, request.CashAccount));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapGet($"{Route}/stores/{{storeFrontId:guid}}/sessions/open", async (Guid storeFrontId, ISender sender) =>
        {
            var result = await sender.Send(new GetOpenPosCashierSessionQuery(storeFrontId));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapGet($"{Route}/stores/{{storeFrontId:guid}}/sessions/open-targets", async (Guid storeFrontId, ISender sender) =>
        {
            var result = await sender.Send(new GetOpenPosCashierSessionsQuery(storeFrontId));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPost($"{Route}/sessions/open", async (OpenPosCashierSessionRequest request, ISender sender) =>
        {
            var result = await sender.Send(new OpenPosCashierSessionCommand(request.Session));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPost($"{Route}/sessions/{{sessionId:guid}}/close", async (Guid sessionId, ClosePosCashierSessionRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ClosePosCashierSessionCommand(sessionId, request.Close));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapGet($"{Route}/stores/{{storeFrontId:guid}}/sessions/summary", async (Guid storeFrontId, DateTime? fromDate, DateTime? toDate, bool? ownOnly, ISender sender) =>
        {
            var result = await sender.Send(new GetPosCashierSessionSummaryQuery(storeFrontId, fromDate, toDate, ownOnly == true));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapGet($"{Route}/stores/{{storeFrontId:guid}}/sessions", async (Guid storeFrontId, DateTime? fromDate, DateTime? toDate, bool? ownOnly, ISender sender) =>
        {
            var result = await sender.Send(new GetPosCashierSessionsQuery(storeFrontId, fromDate, toDate, ownOnly == true));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
