namespace RealEstate.Features;

public record CreatePropertyRequest(PropertyDto Property);
public record UpdatePropertyRequest(PropertyDto Property);
public record CreateUnitRequest(PropertyUnitDto Unit);
public record UpdateUnitRequest(PropertyUnitDto Unit);
public record CreateLeaseRequest(LeaseDto Lease);
public record UpdateLeaseRequest(LeaseDto Lease);
public record RecordRentPaymentRequest(RecordRentPaymentDto Payment);
public record CreateExpenseRequest(PropertyExpenseDto Expense);
public record UpdateExpenseRequest(PropertyExpenseDto Expense);
public record CreateUtilityAccountRequest(UtilityAccountDto UtilityAccount);
public record UpdateUtilityAccountRequest(UtilityAccountDto UtilityAccount);
public record CreateUtilityBillRequest(UtilityBillDto UtilityBill);
public record UpdateUtilityBillRequest(UtilityBillDto UtilityBill);

public record IdResult(Guid Id);
public record BoolResult(bool IsSuccess);

public record GetPropertiesQuery(Guid? CompanyId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetPropertiesResult>;
public record GetPropertiesResult(PaginatedResult<PropertyDto> Properties);
public record GetPropertyByIdQuery(Guid Id) : IQuery<GetPropertyByIdResult>;
public record GetPropertyByIdResult(PropertyDto Property);
public record CreatePropertyCommand(PropertyDto Property) : ICommand<IdResult>;
public record UpdatePropertyCommand(Guid Id, PropertyDto Property) : ICommand<BoolResult>;
public record DeletePropertyCommand(Guid Id) : ICommand<BoolResult>;

public record GetUnitsQuery(Guid? PropertyId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetUnitsResult>;
public record GetUnitsResult(PaginatedResult<PropertyUnitDto> Units);
public record GetUnitByIdQuery(Guid Id) : IQuery<GetUnitByIdResult>;
public record GetUnitByIdResult(PropertyUnitDto Unit);
public record CreateUnitCommand(PropertyUnitDto Unit) : ICommand<IdResult>;
public record UpdateUnitCommand(Guid Id, PropertyUnitDto Unit) : ICommand<BoolResult>;
public record DeleteUnitCommand(Guid Id) : ICommand<BoolResult>;

public record GetLeasesQuery(Guid? CompanyId, LeaseDirection? Direction, Guid? PropertyId, Guid? UnitId, LeaseStatus? Status, int PageIndex, int PageSize, string? SearchText) : IQuery<GetLeasesResult>;
public record GetLeasesResult(PaginatedResult<LeaseDto> Leases);
public record GetLeaseByIdQuery(Guid Id) : IQuery<GetLeaseByIdResult>;
public record GetLeaseByIdResult(LeaseDto Lease);
public record CreateLeaseCommand(LeaseDto Lease) : ICommand<IdResult>;
public record UpdateLeaseCommand(Guid Id, LeaseDto Lease) : ICommand<BoolResult>;
public record GenerateLeaseInstallmentsCommand(Guid Id) : ICommand<BoolResult>;
public record ActivateLeaseCommand(Guid Id) : ICommand<BoolResult>;
public record SuspendLeaseCommand(Guid Id) : ICommand<BoolResult>;
public record TerminateLeaseCommand(Guid Id) : ICommand<BoolResult>;
public record RecordRentPaymentCommand(RecordRentPaymentDto Payment) : ICommand<BoolResult>;

public record GetInstallmentsQuery(Guid? LeaseId, Guid? CompanyId, InstallmentStatus? Status, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize) : IQuery<GetInstallmentsResult>;
public record GetInstallmentsResult(PaginatedResult<LeaseInstallmentDto> Installments);

public record GetExpensesQuery(Guid? CompanyId, Guid? PropertyId, ExpenseCategory? Category, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize) : IQuery<GetExpensesResult>;
public record GetExpensesResult(PaginatedResult<PropertyExpenseDto> Expenses);
public record CreateExpenseCommand(PropertyExpenseDto Expense) : ICommand<IdResult>;
public record UpdateExpenseCommand(Guid Id, PropertyExpenseDto Expense) : ICommand<BoolResult>;
public record DeleteExpenseCommand(Guid Id) : ICommand<BoolResult>;

public record GetUtilityAccountsQuery(Guid? PropertyId, int PageIndex, int PageSize) : IQuery<GetUtilityAccountsResult>;
public record GetUtilityAccountsResult(PaginatedResult<UtilityAccountDto> UtilityAccounts);
public record CreateUtilityAccountCommand(UtilityAccountDto UtilityAccount) : ICommand<IdResult>;
public record UpdateUtilityAccountCommand(Guid Id, UtilityAccountDto UtilityAccount) : ICommand<BoolResult>;
public record DeleteUtilityAccountCommand(Guid Id) : ICommand<BoolResult>;
public record GetUtilityBillsQuery(Guid? PropertyId, Guid? UtilityAccountId, bool? IsPaid, int PageIndex, int PageSize) : IQuery<GetUtilityBillsResult>;
public record GetUtilityBillsResult(PaginatedResult<UtilityBillDto> UtilityBills);
public record CreateUtilityBillCommand(UtilityBillDto UtilityBill) : ICommand<IdResult>;
public record UpdateUtilityBillCommand(Guid Id, UtilityBillDto UtilityBill) : ICommand<BoolResult>;
public record DeleteUtilityBillCommand(Guid Id) : ICommand<BoolResult>;
public record MarkUtilityBillPaidCommand(Guid Id) : ICommand<BoolResult>;

public record GetRealEstateDashboardQuery(Guid? CompanyId) : IQuery<GetRealEstateDashboardResult>;
public record GetRealEstateDashboardResult(RealEstateDashboardDto Dashboard);
public record GetRealEstateReportsQuery(Guid? CompanyId) : IQuery<GetRealEstateReportsResult>;
public record GetRealEstateReportsResult(RealEstateReportsDto Reports);

public class RealEstateEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/real-estate";

        app.MapGet($"{baseRoute}/properties", async (Guid? companyId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetPropertiesQuery(companyId, pageIndex ?? 1, pageSize ?? 20, searchText));
            return Results.Ok(new { properties = result.Properties });
        }).RequireAuthorization(PermissionList.RealEstatePropertyPermissions.View);

        app.MapGet($"{baseRoute}/properties/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(new { property = (await sender.Send(new GetPropertyByIdQuery(id))).Property }))
            .RequireAuthorization(PermissionList.RealEstatePropertyPermissions.View);

        app.MapPost($"{baseRoute}/properties", async (CreatePropertyRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/properties", await sender.Send(new CreatePropertyCommand(request.Property))))
            .RequireAuthorization(PermissionList.RealEstatePropertyPermissions.Create);

        app.MapPut($"{baseRoute}/properties/{{id:guid}}", async (Guid id, UpdatePropertyRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdatePropertyCommand(id, request.Property))))
            .RequireAuthorization(PermissionList.RealEstatePropertyPermissions.Edit);

        app.MapDelete($"{baseRoute}/properties/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeletePropertyCommand(id))))
            .RequireAuthorization(PermissionList.RealEstatePropertyPermissions.Delete);

        app.MapGet($"{baseRoute}/units", async (Guid? propertyId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetUnitsQuery(propertyId, pageIndex ?? 1, pageSize ?? 20, searchText));
            return Results.Ok(new { units = result.Units });
        }).RequireAuthorization(PermissionList.RealEstateUnitPermissions.View);

        app.MapGet($"{baseRoute}/units/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(new { unit = (await sender.Send(new GetUnitByIdQuery(id))).Unit }))
            .RequireAuthorization(PermissionList.RealEstateUnitPermissions.View);

        app.MapPost($"{baseRoute}/units", async (CreateUnitRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/units", await sender.Send(new CreateUnitCommand(request.Unit))))
            .RequireAuthorization(PermissionList.RealEstateUnitPermissions.Create);

        app.MapPut($"{baseRoute}/units/{{id:guid}}", async (Guid id, UpdateUnitRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateUnitCommand(id, request.Unit))))
            .RequireAuthorization(PermissionList.RealEstateUnitPermissions.Edit);

        app.MapDelete($"{baseRoute}/units/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteUnitCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateUnitPermissions.Delete);

        app.MapGet($"{baseRoute}/leases", async (Guid? companyId, LeaseDirection? direction, Guid? propertyId, Guid? unitId, LeaseStatus? status, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetLeasesQuery(companyId, direction, propertyId, unitId, status, pageIndex ?? 1, pageSize ?? 20, searchText));
            return Results.Ok(new { leases = result.Leases });
        }).RequireAuthorization(PermissionList.RealEstateLeasePermissions.View);

        app.MapGet($"{baseRoute}/leases/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(new { lease = (await sender.Send(new GetLeaseByIdQuery(id))).Lease }))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.View);

        app.MapPost($"{baseRoute}/leases", async (CreateLeaseRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/leases", await sender.Send(new CreateLeaseCommand(request.Lease))))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.Create);

        app.MapPut($"{baseRoute}/leases/{{id:guid}}", async (Guid id, UpdateLeaseRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateLeaseCommand(id, request.Lease))))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.Edit);

        app.MapPost($"{baseRoute}/leases/{{id:guid}}/generate-installments", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new GenerateLeaseInstallmentsCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateInstallmentPermissions.Generate);

        app.MapPost($"{baseRoute}/leases/{{id:guid}}/activate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new ActivateLeaseCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.Activate);

        app.MapPost($"{baseRoute}/leases/{{id:guid}}/suspend", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SuspendLeaseCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.Suspend);

        app.MapPost($"{baseRoute}/leases/{{id:guid}}/terminate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new TerminateLeaseCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateLeasePermissions.Terminate);

        app.MapGet($"{baseRoute}/installments", async (Guid? leaseId, Guid? companyId, InstallmentStatus? status, DateTime? fromDate, DateTime? toDate, int? pageIndex, int? pageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetInstallmentsQuery(leaseId, companyId, status, fromDate, toDate, pageIndex ?? 1, pageSize ?? 20));
            return Results.Ok(new { installments = result.Installments });
        }).RequireAuthorization(PermissionList.RealEstateInstallmentPermissions.View);

        app.MapPost($"{baseRoute}/installments/payments", async (RecordRentPaymentRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new RecordRentPaymentCommand(request.Payment))))
            .RequireAuthorization(PermissionList.RealEstateInstallmentPermissions.RecordPayment);

        app.MapGet($"{baseRoute}/expenses", async (Guid? companyId, Guid? propertyId, ExpenseCategory? category, DateTime? fromDate, DateTime? toDate, int? pageIndex, int? pageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetExpensesQuery(companyId, propertyId, category, fromDate, toDate, pageIndex ?? 1, pageSize ?? 20));
            return Results.Ok(new { expenses = result.Expenses });
        }).RequireAuthorization(PermissionList.RealEstateExpensePermissions.View);

        app.MapPost($"{baseRoute}/expenses", async (CreateExpenseRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/expenses", await sender.Send(new CreateExpenseCommand(request.Expense))))
            .RequireAuthorization(PermissionList.RealEstateExpensePermissions.Create);

        app.MapPut($"{baseRoute}/expenses/{{id:guid}}", async (Guid id, UpdateExpenseRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateExpenseCommand(id, request.Expense))))
            .RequireAuthorization(PermissionList.RealEstateExpensePermissions.Edit);

        app.MapDelete($"{baseRoute}/expenses/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteExpenseCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateExpensePermissions.Delete);

        app.MapGet($"{baseRoute}/utility-accounts", async (Guid? propertyId, int? pageIndex, int? pageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetUtilityAccountsQuery(propertyId, pageIndex ?? 1, pageSize ?? 20));
            return Results.Ok(new { utilityAccounts = result.UtilityAccounts });
        }).RequireAuthorization(PermissionList.RealEstateUtilityPermissions.View);

        app.MapPost($"{baseRoute}/utility-accounts", async (CreateUtilityAccountRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/utility-accounts", await sender.Send(new CreateUtilityAccountCommand(request.UtilityAccount))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Create);

        app.MapPut($"{baseRoute}/utility-accounts/{{id:guid}}", async (Guid id, UpdateUtilityAccountRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateUtilityAccountCommand(id, request.UtilityAccount))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Edit);

        app.MapDelete($"{baseRoute}/utility-accounts/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteUtilityAccountCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Delete);

        app.MapGet($"{baseRoute}/utility-bills", async (Guid? propertyId, Guid? utilityAccountId, bool? isPaid, int? pageIndex, int? pageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetUtilityBillsQuery(propertyId, utilityAccountId, isPaid, pageIndex ?? 1, pageSize ?? 20));
            return Results.Ok(new { utilityBills = result.UtilityBills });
        }).RequireAuthorization(PermissionList.RealEstateUtilityPermissions.View);

        app.MapPost($"{baseRoute}/utility-bills", async (CreateUtilityBillRequest request, ISender sender) =>
            Results.Created($"{baseRoute}/utility-bills", await sender.Send(new CreateUtilityBillCommand(request.UtilityBill))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Create);

        app.MapPut($"{baseRoute}/utility-bills/{{id:guid}}", async (Guid id, UpdateUtilityBillRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateUtilityBillCommand(id, request.UtilityBill))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Edit);

        app.MapDelete($"{baseRoute}/utility-bills/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteUtilityBillCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Delete);

        app.MapPost($"{baseRoute}/utility-bills/{{id:guid}}/mark-paid", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new MarkUtilityBillPaidCommand(id))))
            .RequireAuthorization(PermissionList.RealEstateUtilityPermissions.Edit);

        app.MapGet($"{baseRoute}/dashboard", async (Guid? companyId, ISender sender) =>
            Results.Ok(new { dashboard = (await sender.Send(new GetRealEstateDashboardQuery(companyId))).Dashboard }))
            .RequireAuthorization(PermissionList.RealEstateReportsPermissions.View);

        app.MapGet($"{baseRoute}/reports", async (Guid? companyId, ISender sender) =>
            Results.Ok(new { reports = (await sender.Send(new GetRealEstateReportsQuery(companyId))).Reports }))
            .RequireAuthorization(PermissionList.RealEstateReportsPermissions.View);
    }
}

public class RealEstateHandlers(RealEstateDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetPropertiesQuery, GetPropertiesResult>,
    IQueryHandler<GetPropertyByIdQuery, GetPropertyByIdResult>,
    ICommandHandler<CreatePropertyCommand, IdResult>,
    ICommandHandler<UpdatePropertyCommand, BoolResult>,
    ICommandHandler<DeletePropertyCommand, BoolResult>,
    IQueryHandler<GetUnitsQuery, GetUnitsResult>,
    IQueryHandler<GetUnitByIdQuery, GetUnitByIdResult>,
    ICommandHandler<CreateUnitCommand, IdResult>,
    ICommandHandler<UpdateUnitCommand, BoolResult>,
    ICommandHandler<DeleteUnitCommand, BoolResult>,
    IQueryHandler<GetLeasesQuery, GetLeasesResult>,
    IQueryHandler<GetLeaseByIdQuery, GetLeaseByIdResult>,
    ICommandHandler<CreateLeaseCommand, IdResult>,
    ICommandHandler<UpdateLeaseCommand, BoolResult>,
    ICommandHandler<GenerateLeaseInstallmentsCommand, BoolResult>,
    ICommandHandler<ActivateLeaseCommand, BoolResult>,
    ICommandHandler<SuspendLeaseCommand, BoolResult>,
    ICommandHandler<TerminateLeaseCommand, BoolResult>,
    ICommandHandler<RecordRentPaymentCommand, BoolResult>,
    IQueryHandler<GetInstallmentsQuery, GetInstallmentsResult>,
    IQueryHandler<GetExpensesQuery, GetExpensesResult>,
    ICommandHandler<CreateExpenseCommand, IdResult>,
    ICommandHandler<UpdateExpenseCommand, BoolResult>,
    ICommandHandler<DeleteExpenseCommand, BoolResult>,
    IQueryHandler<GetUtilityAccountsQuery, GetUtilityAccountsResult>,
    ICommandHandler<CreateUtilityAccountCommand, IdResult>,
    ICommandHandler<UpdateUtilityAccountCommand, BoolResult>,
    ICommandHandler<DeleteUtilityAccountCommand, BoolResult>,
    IQueryHandler<GetUtilityBillsQuery, GetUtilityBillsResult>,
    ICommandHandler<CreateUtilityBillCommand, IdResult>,
    ICommandHandler<UpdateUtilityBillCommand, BoolResult>,
    ICommandHandler<DeleteUtilityBillCommand, BoolResult>,
    ICommandHandler<MarkUtilityBillPaidCommand, BoolResult>,
    IQueryHandler<GetRealEstateDashboardQuery, GetRealEstateDashboardResult>,
    IQueryHandler<GetRealEstateReportsQuery, GetRealEstateReportsResult>
{
    public async Task<GetPropertiesResult> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Properties.Include(x => x.Units).AsNoTracking().Where(x => !x.IsDeleted);
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.NameEng.Contains(search) || x.Code.Contains(search));
        }
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetPropertiesResult(new PaginatedResult<PropertyDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetPropertyByIdResult> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await dbContext.Properties.Include(x => x.Units).FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Property not found: {request.Id}");
        return new GetPropertyByIdResult(property.ToDto());
    }

    public async Task<IdResult> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId();
        var property = Property.Create(request.Property, userId);
        if (string.IsNullOrWhiteSpace(property.Code))
            request.Property.Code = await GenerateNumberAsync("PROP", request.Property.CompanyId, cancellationToken);
        property = Property.Create(request.Property, userId);
        dbContext.Properties.Add(property);
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpsertMaintenanceAssetAsync(property, cancellationToken);
        return new IdResult(property.Id);
    }

    public async Task<BoolResult> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.Properties.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Property not found: {request.Id}");
        request.Property.Id = request.Id;
        property.Update(request.Property, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpsertMaintenanceAssetAsync(property, cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.Properties.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Property not found: {request.Id}");
        property.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<GetUnitsResult> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PropertyUnits.Include(x => x.Property).AsNoTracking().Where(x => !x.IsDeleted);
        if (request.PropertyId.HasValue) query = query.Where(x => x.PropertyId == request.PropertyId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.UnitNumber.Contains(search) || (x.Name != null && x.Name.Contains(search)));
        }
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetUnitsResult(new PaginatedResult<PropertyUnitDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetUnitByIdResult> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await dbContext.PropertyUnits.Include(x => x.Property).FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Unit not found: {request.Id}");
        return new GetUnitByIdResult(unit.ToDto());
    }

    public async Task<IdResult> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(request.Unit.PropertyId, cancellationToken);
        var unit = PropertyUnit.Create(request.Unit, CurrentUserId());
        dbContext.PropertyUnits.Add(unit);
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpsertMaintenanceAssetAsync(unit, cancellationToken);
        return new IdResult(unit.Id);
    }

    public async Task<BoolResult> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await dbContext.PropertyUnits.Include(x => x.Property).FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Unit not found: {request.Id}");
        request.Unit.Id = request.Id;
        unit.Update(request.Unit, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpsertMaintenanceAssetAsync(unit, cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await dbContext.PropertyUnits.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Unit not found: {request.Id}");
        unit.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<GetLeasesResult> Handle(GetLeasesQuery request, CancellationToken cancellationToken)
    {
        var query = LeaseQuery().AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.Direction.HasValue) query = query.Where(x => x.Direction == request.Direction);
        if (request.PropertyId.HasValue) query = query.Where(x => x.PropertyId == request.PropertyId);
        if (request.UnitId.HasValue) query = query.Where(x => x.UnitId == request.UnitId);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Number.Contains(search) || x.PartyDisplayName.Contains(search) || x.ContractNumber.Contains(search));
        }
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetLeasesResult(new PaginatedResult<LeaseDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetLeaseByIdResult> Handle(GetLeaseByIdQuery request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        return new GetLeaseByIdResult(lease.ToDto());
    }

    public async Task<IdResult> Handle(CreateLeaseCommand request, CancellationToken cancellationToken)
    {
        await ValidateLeaseReferencesAsync(request.Lease, cancellationToken);
        await EnsureNoOverlapAsync(request.Lease, null, cancellationToken);
        var number = string.IsNullOrWhiteSpace(request.Lease.Number)
            ? await GenerateNumberAsync(request.Lease.Direction == LeaseDirection.OwnerToCompany ? "OWNL" : "TENL", request.Lease.CompanyId, cancellationToken)
            : request.Lease.Number.Trim();
        var lease = Lease.Create(number, request.Lease, CurrentUserId());
        dbContext.Leases.Add(lease);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new IdResult(lease.Id);
    }

    public async Task<BoolResult> Handle(UpdateLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        request.Lease.Id = request.Id;
        await ValidateLeaseReferencesAsync(request.Lease, cancellationToken);
        await EnsureNoOverlapAsync(request.Lease, request.Id, cancellationToken);
        lease.Update(request.Lease, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(GenerateLeaseInstallmentsCommand request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        lease.GenerateInstallments(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(ActivateLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        await EnsureContractCanActivateAsync(lease, cancellationToken);
        await EnsureNoOverlapAsync(lease.ToDto(), lease.Id, cancellationToken);
        lease.Activate(CurrentUserId());
        if (lease.Direction == LeaseDirection.CompanyToTenant && lease.UnitId.HasValue)
        {
            lease.Unit?.ChangeStatus(UnitStatus.Occupied, CurrentUserId());
            dbContext.OccupancyHistory.Add(OccupancyHistory.Create(lease.PropertyId, lease.UnitId.Value, lease.Id, lease.PartyId, lease.StartDate, CurrentUserId()));
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(SuspendLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await dbContext.Leases.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        lease.Suspend(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(TerminateLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Id}");
        lease.Terminate(CurrentUserId());
        if (lease.Direction == LeaseDirection.CompanyToTenant && lease.Unit is not null)
        {
            lease.Unit.ChangeStatus(UnitStatus.Available, CurrentUserId());
            var occupancy = await dbContext.OccupancyHistory.FirstOrDefaultAsync(x => x.LeaseId == lease.Id && !x.EndDate.HasValue, cancellationToken);
            occupancy?.Close(DateTime.UtcNow, CurrentUserId());
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(RecordRentPaymentCommand request, CancellationToken cancellationToken)
    {
        var lease = await LeaseQuery().FirstOrDefaultAsync(x => x.Id == request.Payment.LeaseId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lease not found: {request.Payment.LeaseId}");
        if (lease.Direction != LeaseDirection.CompanyToTenant)
            throw new BadRequestException("Rent collection can only be recorded for tenant leases.");

        var installments = lease.Installments.ToDictionary(x => x.Id);
        foreach (var allocation in request.Payment.Allocations)
        {
            if (!installments.TryGetValue(allocation.InstallmentId, out var installment))
                throw new BadRequestException("Payment allocation installment does not belong to the lease.");
            installment.AllocatePayment(request.Payment.PaymentReferenceId, request.Payment.PaymentDate, request.Payment.Reference, allocation.Amount, request.Payment.Notes, CurrentUserId());
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        var paidAmount = request.Payment.Allocations.Sum(x => x.Amount);
        if (paidAmount > 0)
        {
            await sender.Send(new RecordAccountingReceiptCommand(
                lease.CompanyId,
                lease.BranchId,
                lease.PartyId,
                lease.PartyDisplayName,
                "RealEstate",
                request.Payment.PaymentReferenceId,
                request.Payment.Reference ?? lease.Number,
                paidAmount,
                false,
                request.Payment.PaymentDate), cancellationToken);
        }
        return new BoolResult(true);
    }

    public async Task<GetInstallmentsResult> Handle(GetInstallmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.LeaseInstallments.AsNoTracking().Where(x => !x.IsDeleted);
        if (request.LeaseId.HasValue) query = query.Where(x => x.LeaseId == request.LeaseId);
        if (request.CompanyId.HasValue) query = query.Where(x => dbContext.Leases.Any(l => l.Id == x.LeaseId && l.CompanyId == request.CompanyId));
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        if (request.FromDate.HasValue) query = query.Where(x => x.DueDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.DueDate <= request.ToDate.Value.Date);
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.DueDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetInstallmentsResult(new PaginatedResult<LeaseInstallmentDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetExpensesResult> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PropertyExpenses.Include(x => x.Property).AsNoTracking().Where(x => !x.IsDeleted);
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.PropertyId.HasValue) query = query.Where(x => x.PropertyId == request.PropertyId);
        if (request.Category.HasValue) query = query.Where(x => x.Category == request.Category);
        if (request.FromDate.HasValue) query = query.Where(x => x.ExpenseDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.ExpenseDate <= request.ToDate.Value.Date);
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.ExpenseDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetExpensesResult(new PaginatedResult<PropertyExpenseDto>(pageIndex, pageSize, count, data));
    }

    public async Task<IdResult> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(request.Expense.PropertyId, cancellationToken);
        var expense = PropertyExpense.Create(request.Expense, CurrentUserId());
        dbContext.PropertyExpenses.Add(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        await PostPropertyExpenseAccountingAsync(expense, cancellationToken);
        return new IdResult(expense.Id);
    }

    public async Task<BoolResult> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(request.Expense.PropertyId, cancellationToken);
        var expense = await dbContext.PropertyExpenses.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Expense not found: {request.Id}");
        request.Expense.Id = request.Id;
        expense.Update(request.Expense, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.PropertyExpenses.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Expense not found: {request.Id}");
        expense.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<GetUtilityAccountsResult> Handle(GetUtilityAccountsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.UtilityAccounts.AsNoTracking().Where(x => !x.IsDeleted);
        if (request.PropertyId.HasValue) query = query.Where(x => x.PropertyId == request.PropertyId);
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetUtilityAccountsResult(new PaginatedResult<UtilityAccountDto>(pageIndex, pageSize, count, data));
    }

    public async Task<IdResult> Handle(CreateUtilityAccountCommand request, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(request.UtilityAccount.PropertyId, cancellationToken);
        var account = UtilityAccount.Create(request.UtilityAccount, CurrentUserId());
        dbContext.UtilityAccounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new IdResult(account.Id);
    }

    public async Task<BoolResult> Handle(UpdateUtilityAccountCommand request, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(request.UtilityAccount.PropertyId, cancellationToken);
        var account = await dbContext.UtilityAccounts.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility account not found: {request.Id}");
        request.UtilityAccount.Id = request.Id;
        account.Update(request.UtilityAccount, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(DeleteUtilityAccountCommand request, CancellationToken cancellationToken)
    {
        var hasBills = await dbContext.UtilityBills.AnyAsync(x => x.UtilityAccountId == request.Id && !x.IsDeleted, cancellationToken);
        if (hasBills)
            throw new BadRequestException("Utility account has bills and cannot be deleted.");

        var account = await dbContext.UtilityAccounts.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility account not found: {request.Id}");
        account.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<GetUtilityBillsResult> Handle(GetUtilityBillsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.UtilityBills.AsNoTracking().Where(x => !x.IsDeleted);
        if (request.PropertyId.HasValue) query = query.Where(x => x.PropertyId == request.PropertyId);
        if (request.UtilityAccountId.HasValue) query = query.Where(x => x.UtilityAccountId == request.UtilityAccountId);
        if (request.IsPaid.HasValue) query = query.Where(x => x.IsPaid == request.IsPaid);
        var (pageIndex, pageSize) = Page(request.PageIndex, request.PageSize);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.DueDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new GetUtilityBillsResult(new PaginatedResult<UtilityBillDto>(pageIndex, pageSize, count, data));
    }

    public async Task<IdResult> Handle(CreateUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var account = await dbContext.UtilityAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UtilityBill.UtilityAccountId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility account not found: {request.UtilityBill.UtilityAccountId}");
        request.UtilityBill.PropertyId = account.PropertyId;
        request.UtilityBill.UnitId = account.UnitId;
        var bill = UtilityBill.Create(request.UtilityBill, CurrentUserId());
        dbContext.UtilityBills.Add(bill);
        await dbContext.SaveChangesAsync(cancellationToken);
        await PostUtilityBillAccountingAsync(bill, account, cancellationToken);
        return new IdResult(bill.Id);
    }

    public async Task<BoolResult> Handle(UpdateUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var account = await dbContext.UtilityAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UtilityBill.UtilityAccountId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility account not found: {request.UtilityBill.UtilityAccountId}");
        var bill = await dbContext.UtilityBills.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility bill not found: {request.Id}");
        request.UtilityBill.Id = request.Id;
        request.UtilityBill.PropertyId = account.PropertyId;
        request.UtilityBill.UnitId = account.UnitId;
        bill.Update(request.UtilityBill, CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(DeleteUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var bill = await dbContext.UtilityBills.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility bill not found: {request.Id}");
        bill.Remove(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BoolResult(true);
    }

    public async Task<BoolResult> Handle(MarkUtilityBillPaidCommand request, CancellationToken cancellationToken)
    {
        var bill = await dbContext.UtilityBills.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility bill not found: {request.Id}");
        bill.MarkPaid(CurrentUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        var account = await dbContext.UtilityAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bill.UtilityAccountId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Utility account not found: {bill.UtilityAccountId}");
        await PostUtilityBillPaymentAccountingAsync(bill, account, cancellationToken);
        return new BoolResult(true);
    }

    public async Task<GetRealEstateDashboardResult> Handle(GetRealEstateDashboardQuery request, CancellationToken cancellationToken)
    {
        var properties = dbContext.Properties.AsNoTracking().Where(x => !x.IsDeleted);
        var units = dbContext.PropertyUnits.AsNoTracking().Where(x => !x.IsDeleted);
        var leases = dbContext.Leases.AsNoTracking().Where(x => !x.IsDeleted);
        var installments = dbContext.LeaseInstallments.AsNoTracking().Where(x => !x.IsDeleted);
        var expenses = dbContext.PropertyExpenses.AsNoTracking().Where(x => !x.IsDeleted);
        if (request.CompanyId.HasValue)
        {
            properties = properties.Where(x => x.CompanyId == request.CompanyId);
            leases = leases.Where(x => x.CompanyId == request.CompanyId);
            expenses = expenses.Where(x => x.CompanyId == request.CompanyId);
            units = units.Where(x => properties.Select(p => p.Id).Contains(x.PropertyId));
            installments = installments.Where(x => leases.Select(l => l.Id).Contains(x.LeaseId));
        }

        var expectedRent = await installments.SumAsync(x => x.Amount, cancellationToken);
        var collectedRent = await installments.SumAsync(x => x.PaidAmount, cancellationToken);
        var overdueRent = await installments.Where(x => x.Status == InstallmentStatus.Overdue || (x.Status == InstallmentStatus.Pending && x.DueDate < DateTime.UtcNow.Date)).SumAsync(x => x.Amount - x.PaidAmount, cancellationToken);
        var expenseTotal = await expenses.SumAsync(x => x.Amount + x.TaxAmount, cancellationToken);
        return new GetRealEstateDashboardResult(new RealEstateDashboardDto
        {
            Properties = await properties.CountAsync(cancellationToken),
            Units = await units.CountAsync(cancellationToken),
            OccupiedUnits = await units.CountAsync(x => x.Status == UnitStatus.Occupied, cancellationToken),
            ActiveTenantLeases = await leases.CountAsync(x => x.Direction == LeaseDirection.CompanyToTenant && x.Status == LeaseStatus.Active, cancellationToken),
            ExpectedRent = expectedRent,
            CollectedRent = collectedRent,
            OverdueRent = overdueRent,
            Expenses = expenseTotal,
            NetProfit = collectedRent - expenseTotal
        });
    }

    public async Task<GetRealEstateReportsResult> Handle(GetRealEstateReportsQuery request, CancellationToken cancellationToken)
    {
        var propertiesQuery = dbContext.Properties.AsNoTracking().Where(x => !x.IsDeleted);
        var unitsQuery = dbContext.PropertyUnits.AsNoTracking().Where(x => !x.IsDeleted);
        var leasesQuery = dbContext.Leases.AsNoTracking().Where(x => !x.IsDeleted);
        var installmentsQuery = dbContext.LeaseInstallments.AsNoTracking().Where(x => !x.IsDeleted);
        var expensesQuery = dbContext.PropertyExpenses.AsNoTracking().Where(x => !x.IsDeleted);
        var utilityBillsQuery = dbContext.UtilityBills.AsNoTracking().Where(x => !x.IsDeleted);

        if (request.CompanyId.HasValue)
        {
            propertiesQuery = propertiesQuery.Where(x => x.CompanyId == request.CompanyId);
            leasesQuery = leasesQuery.Where(x => x.CompanyId == request.CompanyId);
            expensesQuery = expensesQuery.Where(x => x.CompanyId == request.CompanyId);
            unitsQuery = unitsQuery.Where(x => propertiesQuery.Select(p => p.Id).Contains(x.PropertyId));
            installmentsQuery = installmentsQuery.Where(x => leasesQuery.Select(l => l.Id).Contains(x.LeaseId));
            utilityBillsQuery = utilityBillsQuery.Where(x => propertiesQuery.Select(p => p.Id).Contains(x.PropertyId));
        }

        var properties = await propertiesQuery.Select(x => new { x.Id, x.Name }).ToListAsync(cancellationToken);
        var units = await unitsQuery.Select(x => new { x.PropertyId, x.Status }).ToListAsync(cancellationToken);
        var leases = await leasesQuery.Select(x => new { x.Id, x.PropertyId, x.Direction }).ToListAsync(cancellationToken);
        var installments = await installmentsQuery.Select(x => new LeaseInstallmentDto
        {
            Id = x.Id,
            LeaseId = x.LeaseId,
            Sequence = x.Sequence,
            DueDate = x.DueDate,
            PeriodStart = x.PeriodStart,
            PeriodEnd = x.PeriodEnd,
            Amount = x.Amount,
            PaidAmount = x.PaidAmount,
            RemainingAmount = x.Amount - x.PaidAmount,
            Status = x.Status,
            Notes = x.Notes
        }).ToListAsync(cancellationToken);
        var expenses = await expensesQuery.Select(x => new { x.PropertyId, x.Category, x.Amount, x.TaxAmount }).ToListAsync(cancellationToken);

        var leaseProperty = leases.ToDictionary(x => x.Id, x => x.PropertyId);
        var tenantLeaseIds = leases.Where(x => x.Direction == LeaseDirection.CompanyToTenant).Select(x => x.Id).ToHashSet();
        var ownerLeaseIds = leases.Where(x => x.Direction == LeaseDirection.OwnerToCompany).Select(x => x.Id).ToHashSet();

        var reports = new RealEstateReportsDto
        {
            Profitability = properties.Select(property =>
            {
                var propertyLeaseIds = leaseProperty.Where(x => x.Value == property.Id).Select(x => x.Key).ToHashSet();
                var propertyInstallments = installments.Where(x => propertyLeaseIds.Contains(x.LeaseId) && tenantLeaseIds.Contains(x.LeaseId)).ToList();
                var propertyExpenses = expenses.Where(x => x.PropertyId == property.Id).ToList();
                var expenseTotal = propertyExpenses.Sum(x => x.Amount + x.TaxAmount);
                var collected = propertyInstallments.Sum(x => x.PaidAmount);
                return new PropertyProfitabilityDto
                {
                    PropertyId = property.Id,
                    PropertyName = property.Name,
                    ExpectedRent = propertyInstallments.Sum(x => x.Amount),
                    CollectedRent = collected,
                    Expenses = expenseTotal,
                    NetProfit = collected - expenseTotal
                };
            }).OrderByDescending(x => x.NetProfit).ToList(),
            Occupancy = properties.Select(property =>
            {
                var propertyUnits = units.Where(x => x.PropertyId == property.Id).ToList();
                var occupied = propertyUnits.Count(x => x.Status == UnitStatus.Occupied);
                return new PropertyOccupancyDto
                {
                    PropertyId = property.Id,
                    PropertyName = property.Name,
                    Units = propertyUnits.Count,
                    OccupiedUnits = occupied,
                    OccupancyRate = propertyUnits.Count == 0 ? 0 : Math.Round((decimal)occupied / propertyUnits.Count * 100, 2)
                };
            }).OrderByDescending(x => x.OccupancyRate).ToList(),
            OverdueInstallments = installments
                .Where(x => tenantLeaseIds.Contains(x.LeaseId) && (x.Status == InstallmentStatus.Overdue || (x.Status == InstallmentStatus.Pending && x.DueDate.Date < DateTime.UtcNow.Date)))
                .OrderBy(x => x.DueDate)
                .Take(50)
                .ToList(),
            UpcomingOwnerPayables = installments
                .Where(x => ownerLeaseIds.Contains(x.LeaseId) && (x.Status == InstallmentStatus.Pending || x.Status == InstallmentStatus.PartiallyPaid) && x.DueDate.Date <= DateTime.UtcNow.Date.AddDays(30))
                .OrderBy(x => x.DueDate)
                .Take(50)
                .ToList(),
            UtilityAging = await utilityBillsQuery
                .Where(x => !x.IsPaid && x.DueDate <= DateTime.UtcNow.Date.AddDays(30))
                .OrderBy(x => x.DueDate)
                .Take(50)
                .Select(x => x.ToDto())
                .ToListAsync(cancellationToken),
            ExpenseSummary = expenses
                .GroupBy(x => x.Category)
                .Select(x => new ExpenseSummaryDto
                {
                    Category = x.Key,
                    Amount = x.Sum(e => e.Amount),
                    TaxAmount = x.Sum(e => e.TaxAmount),
                    TotalAmount = x.Sum(e => e.Amount + e.TaxAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList()
        };

        return new GetRealEstateReportsResult(reports);
    }

    private IQueryable<Lease> LeaseQuery() => dbContext.Leases.Include(x => x.Property).Include(x => x.Unit).Include("Installments").Where(x => !x.IsDeleted);

    private async Task ValidateLeaseReferencesAsync(LeaseDto lease, CancellationToken cancellationToken)
    {
        await EnsurePropertyAsync(lease.PropertyId, cancellationToken);
        if (lease.UnitId.HasValue)
        {
            var unitExists = await dbContext.PropertyUnits.AnyAsync(x => x.Id == lease.UnitId && x.PropertyId == lease.PropertyId && !x.IsDeleted, cancellationToken);
            if (!unitExists) throw new BadRequestException("Unit does not belong to the selected property.");
        }
        var expectedPartyType = lease.Direction == LeaseDirection.OwnerToCompany ? "Supplier" : "Customer";
        if (!string.Equals(lease.PartyType, expectedPartyType, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException($"Lease party type must be {expectedPartyType}.");
    }

    private async Task EnsureContractCanActivateAsync(Lease lease, CancellationToken cancellationToken)
    {
        var expectedType = lease.Direction == LeaseDirection.OwnerToCompany ? "RealEstate.OwnerLease" : "RealEstate.TenantLease";
        var result = await sender.Send(new GetActiveContractStatusQuery(lease.PartyType, lease.PartyId, lease.CompanyId, expectedType), cancellationToken);
        if (result.ContractId != lease.ContractId || result.Status is not (ContractStatus.Active or ContractStatus.Signed))
            throw new BadRequestException("A signed or active matching real estate contract is required before lease activation.");
    }

    private async Task EnsureNoOverlapAsync(LeaseDto lease, Guid? currentLeaseId, CancellationToken cancellationToken)
    {
        var activeStatuses = new[] { LeaseStatus.Active, LeaseStatus.Suspended };
        var query = dbContext.Leases.AsNoTracking()
            .Where(x => !x.IsDeleted && activeStatuses.Contains(x.Status) && x.Direction == lease.Direction && x.StartDate <= lease.EndDate.Date && x.EndDate >= lease.StartDate.Date);
        if (currentLeaseId.HasValue) query = query.Where(x => x.Id != currentLeaseId.Value);

        var hasOverlap = lease.Direction == LeaseDirection.CompanyToTenant
            ? await query.AnyAsync(x => x.UnitId == lease.UnitId, cancellationToken)
            : await query.AnyAsync(x => x.PropertyId == lease.PropertyId, cancellationToken);
        if (hasOverlap)
            throw new BadRequestException("An active lease already overlaps this property/unit date range.");
    }

    private async Task EnsurePropertyAsync(Guid propertyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Properties.AnyAsync(x => x.Id == propertyId && !x.IsDeleted, cancellationToken);
        if (!exists) throw new NotFoundException($"Property not found: {propertyId}");
    }

    private async Task PostPropertyExpenseAccountingAsync(PropertyExpense expense, CancellationToken cancellationToken)
    {
        var document = new AccountingDocumentDto
        {
            CompanyId = expense.CompanyId,
            Type = AccountingDocumentType.SupplierInvoice,
            DocumentDate = expense.ExpenseDate,
            PartyId = expense.SupplierId,
            PartyName = expense.SupplierName,
            SourceModule = "RealEstate",
            SourceDocumentId = expense.Id,
            SourceDocumentNumber = expense.SourceDocumentNumber ?? $"EXP-{expense.Id:N}"[..16],
            Lines =
            [
                new AccountingDocumentLineDto
                {
                    Description = expense.Category.ToString(),
                    Quantity = 1,
                    UnitPrice = expense.Amount,
                    NetAmount = expense.Amount,
                    TaxAmount = expense.TaxAmount,
                    TotalAmount = expense.TotalAmount
                }
            ]
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(document), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
    }

    private async Task PostUtilityBillAccountingAsync(UtilityBill bill, UtilityAccount account, CancellationToken cancellationToken)
    {
        var document = new AccountingDocumentDto
        {
            CompanyId = account.CompanyId,
            Type = AccountingDocumentType.SupplierInvoice,
            DocumentDate = bill.BillDate,
            PartyId = account.SupplierId,
            PartyName = account.ProviderName,
            SourceModule = "RealEstate",
            SourceDocumentId = bill.Id,
            SourceDocumentNumber = bill.Reference ?? $"UTIL-{bill.Id:N}"[..17],
            Lines =
            [
                new AccountingDocumentLineDto
                {
                    Description = $"{account.ServiceType} utility bill",
                    Quantity = 1,
                    UnitPrice = bill.Amount,
                    NetAmount = bill.Amount,
                    TaxAmount = bill.TaxAmount,
                    TotalAmount = bill.TotalAmount
                }
            ]
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(document), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
    }

    private async Task PostUtilityBillPaymentAccountingAsync(UtilityBill bill, UtilityAccount account, CancellationToken cancellationToken)
    {
        var document = new AccountingDocumentDto
        {
            CompanyId = account.CompanyId,
            Type = AccountingDocumentType.SupplierPayment,
            DocumentDate = DateTime.UtcNow,
            PartyId = account.SupplierId,
            PartyName = account.ProviderName,
            SourceModule = "RealEstate",
            SourceDocumentId = bill.Id,
            SourceDocumentNumber = bill.Reference ?? $"UTIL-PAY-{bill.Id:N}"[..21],
            Lines =
            [
                new AccountingDocumentLineDto
                {
                    Description = $"{account.ServiceType} utility payment",
                    Quantity = 1,
                    UnitPrice = bill.TotalAmount,
                    NetAmount = bill.TotalAmount,
                    TotalAmount = bill.TotalAmount
                }
            ]
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(document), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
    }

    private async Task UpsertMaintenanceAssetAsync(Property property, CancellationToken cancellationToken) =>
        await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "RealEstate",
            "Property",
            property.Id,
            property.Code,
            property.Name,
            property.NameEng,
            MaintenanceAssetType.Building,
            property.Status == PropertyStatus.Inactive ? MaintenanceAssetStatus.Inactive : MaintenanceAssetStatus.Active,
            property.CompanyId,
            property.BranchId,
            null,
            property.Notes,
            property.Address,
            null,
            null,
            null), cancellationToken);

    private async Task UpsertMaintenanceAssetAsync(PropertyUnit unit, CancellationToken cancellationToken)
    {
        var property = unit.Property ?? await dbContext.Properties.AsNoTracking().FirstAsync(x => x.Id == unit.PropertyId, cancellationToken);
        var parent = await sender.Send(new GetLinkedMaintenanceAssetQuery("RealEstate", "Property", unit.PropertyId), cancellationToken);
        await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "RealEstate",
            "PropertyUnit",
            unit.Id,
            unit.UnitNumber,
            unit.Name ?? unit.UnitNumber,
            unit.Name ?? unit.UnitNumber,
            unit.UnitType == PropertyUnitType.Office ? MaintenanceAssetType.Office : MaintenanceAssetType.Apartment,
            unit.Status == UnitStatus.Inactive ? MaintenanceAssetStatus.Inactive : unit.Status == UnitStatus.UnderMaintenance ? MaintenanceAssetStatus.UnderMaintenance : MaintenanceAssetStatus.Active,
            property.CompanyId,
            property.BranchId,
            parent.MaintenanceAssetId,
            unit.Notes,
            property.Address,
            null,
            null,
            null), cancellationToken);
    }

    private async Task<string> GenerateNumberAsync(string prefix, Guid companyId, CancellationToken cancellationToken)
    {
        var count = await dbContext.Leases.CountAsync(x => x.CompanyId == companyId, cancellationToken)
            + await dbContext.Properties.CountAsync(x => x.CompanyId == companyId, cancellationToken) + 1;
        return $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }

    private string CurrentUserId() => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("sub")
        ?? "system";

    private static (int PageIndex, int PageSize) Page(int pageIndex, int pageSize) => (Math.Max(1, pageIndex), Math.Clamp(pageSize, 1, 200));
}
