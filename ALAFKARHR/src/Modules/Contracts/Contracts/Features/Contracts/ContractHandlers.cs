namespace Contracts.Contracts.Features.Contracts;

public class ContractHandlers(ContractsDbContext dbContext, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetContractsQuery, GetContractsResult>,
    IQueryHandler<GetContractByIdQuery, GetContractByIdResult>,
    ICommandHandler<CreateContractCommand, CreateContractResult>,
    ICommandHandler<UpdateContractCommand, UpdateContractResult>,
    ICommandHandler<DeleteContractCommand, DeleteContractResult>,
    ICommandHandler<ChangeContractStatusCommand, ChangeContractStatusResult>,
    ICommandHandler<ConfigureContractRenewalCommand, ConfigureContractRenewalResult>,
    ICommandHandler<ProcessContractRenewalCommand, ProcessContractRenewalResult>,
    ICommandHandler<RecordContractRenewalPaymentCommand, RecordContractRenewalPaymentResult>,
    IQueryHandler<GetPartyContractsQuery, GetPartyContractsResult>,
    IQueryHandler<GetActiveContractStatusQuery, GetActiveContractStatusResult>,
    ICommandHandler<CreateLinkedContractCommand, CreateLinkedContractResult>,
    IQueryHandler<GetContractRenewalObligationsQuery, GetContractRenewalObligationsResult>
{
    public async Task<GetContractsResult> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts.IncludeDetails().AsNoTracking().AsQueryable();

        if (request.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.PartyType))
            query = query.Where(x => x.PartyType == request.PartyType);
        if (request.PartyId.HasValue)
            query = query.Where(x => x.PartyId == request.PartyId);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.Type))
            query = query.Where(x => x.Type == request.Type);
        if (request.FromDate.HasValue)
            query = query.Where(x => x.EndDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.EndDate <= request.ToDate.Value.Date);
        if (request.PaymentStatus.HasValue)
            query = query.Where(x => x.Renewals.Any(r => r.PaymentStatus == request.PaymentStatus.Value));
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Number.Contains(search) || x.Title.Contains(search) || x.PartyDisplayName.Contains(search));
        }

        var pageIndex = Math.Max(1, request.PageIndex);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new GetContractsResult(new PaginatedResult<ContractDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetContractByIdResult> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");

        return new GetContractByIdResult(contract.ToDto());
    }

    public async Task<CreateContractResult> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var userId = ContractFeatureHelpers.CurrentUserId(httpContextAccessor);
        var number = string.IsNullOrWhiteSpace(request.Contract.Number)
            ? await ContractFeatureHelpers.GenerateNumberAsync(dbContext, request.Contract.CompanyId, request.Contract.Type, cancellationToken)
            : request.Contract.Number.Trim();

        var contract = Contract.Create(number, request.Contract, userId);
        await dbContext.Contracts.AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateContractResult(contract.Id, contract.Number);
    }

    public async Task<UpdateContractResult> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");

        contract.Update(request.Contract, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateContractResult(true);
    }

    public async Task<DeleteContractResult> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");
        contract.Remove(ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteContractResult(true);
    }

    public async Task<ChangeContractStatusResult> Handle(ChangeContractStatusCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");

        contract.ChangeStatus(request.Status, request.Action, request.Notes, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ChangeContractStatusResult(true);
    }

    public async Task<ConfigureContractRenewalResult> Handle(ConfigureContractRenewalCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");
        contract.ConfigureRenewal(request.Settings, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ConfigureContractRenewalResult(true);
    }

    public async Task<ProcessContractRenewalResult> Handle(ProcessContractRenewalCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");
        var renewal = contract.ProcessRenewal(ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProcessContractRenewalResult(renewal.ToDto());
    }

    public async Task<RecordContractRenewalPaymentResult> Handle(RecordContractRenewalPaymentCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.Id}");
        contract.ActivatePaidRenewal(request.RenewalId, request.PaymentReferenceId, request.PaidAmount, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RecordContractRenewalPaymentResult(true);
    }

    public async Task<GetPartyContractsResult> Handle(GetPartyContractsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts.IncludeDetails().AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.PartyType == request.PartyType && x.PartyId == request.PartyId);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status);

        var contracts = await query.OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
        return new GetPartyContractsResult(contracts);
    }

    public async Task<GetActiveContractStatusResult> Handle(GetActiveContractStatusQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts.IncludeDetails().AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.PartyType == request.PartyType && x.PartyId == request.PartyId);
        if (!string.IsNullOrWhiteSpace(request.ContractType))
            query = query.Where(x => x.Type == request.ContractType);

        var contract = await query
            .OrderByDescending(x => x.Status == ContractStatus.Active)
            .ThenByDescending(x => x.EndDate)
            .FirstOrDefaultAsync(cancellationToken);

        return contract is null
            ? new GetActiveContractStatusResult(false, null, null, null, null, false)
            : new GetActiveContractStatusResult(
                contract.Status == ContractStatus.Active,
                contract.Id,
                contract.Number,
                contract.Status,
                contract.EndDate,
                contract.Status == ContractStatus.PendingRenewalPayment);
    }

    public async Task<CreateLinkedContractResult> Handle(CreateLinkedContractCommand request, CancellationToken cancellationToken)
    {
        var result = await Handle(new CreateContractCommand(request.Contract), cancellationToken);
        return new CreateLinkedContractResult(result.Id, result.Number);
    }

    public async Task<GetContractRenewalObligationsResult> Handle(GetContractRenewalObligationsQuery request, CancellationToken cancellationToken)
    {
        var obligations = await dbContext.Contracts.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId
                && x.Status == ContractStatus.Active
                && x.RenewalSettings.AutoRenew
                && x.EndDate >= request.FromDate.Date
                && x.EndDate <= request.ToDate.Date)
            .Select(x => new ContractRenewalObligationDto
            {
                ContractId = x.Id,
                ContractNumber = x.Number,
                Title = x.Title,
                PartyType = x.PartyType,
                PartyId = x.PartyId,
                PartyDisplayName = x.PartyDisplayName,
                EndDate = x.EndDate,
                RequiresPayment = x.RenewalSettings.RequiresRenewalFee,
                FeeAmount = x.RenewalSettings.RequiresRenewalFee
                    ? (x.RenewalSettings.FeeMode == ContractRenewalFeeMode.FixedAmount
                        ? x.RenewalSettings.FeeAmount.GetValueOrDefault()
                        : Math.Round(x.ContractValue * x.RenewalSettings.FeePercentage.GetValueOrDefault() / 100m, 2))
                    : 0,
                CurrencyId = x.RenewalSettings.CurrencyId ?? x.CurrencyId
            })
            .ToListAsync(cancellationToken);

        return new GetContractRenewalObligationsResult(obligations);
    }
}
