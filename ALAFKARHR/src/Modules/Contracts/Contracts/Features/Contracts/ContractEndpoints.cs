namespace Contracts.Contracts.Features.Contracts;

public record CreateContractRequest(ContractDto Contract);
public record UpdateContractRequest(ContractDto Contract);
public record ConfigureRenewalRequest(ContractRenewalSettingsDto Settings);
public record RecordRenewalPaymentRequest(Guid? PaymentReferenceId, decimal PaidAmount);

public class ContractEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/contracts/contracts";

        app.MapGet(baseRoute, async (Guid? companyId, string? partyType, Guid? partyId, ContractStatus? status, string? type, ContractRenewalPaymentStatus? paymentStatus, DateTime? fromDate, DateTime? toDate, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetContractsQuery(companyId, partyType, partyId, status, type, paymentStatus, fromDate, toDate, pageIndex ?? 1, pageSize ?? 20, searchText));
            return Results.Ok(new { contracts = result.Contracts });
        })
            .WithName("GetContracts")
            .Produces<PaginatedResult<ContractDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractPermissions.View);

        app.MapGet($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetContractByIdQuery(id));
            return Results.Ok(new { contract = result.Contract });
        })
            .WithName("GetContractById")
            .Produces<ContractDto>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractPermissions.View);

        app.MapPost(baseRoute, async (CreateContractRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateContractCommand(request.Contract));
            return Results.Created($"{baseRoute}/{result.Id}", result);
        })
            .WithName("CreateContract")
            .Produces<CreateContractResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.ContractPermissions.Create);

        app.MapPut($"{baseRoute}/{{id:guid}}", async (Guid id, UpdateContractRequest request, ISender sender) =>
        {
            await sender.Send(new UpdateContractCommand(id, request.Contract));
            return Results.Ok("OK");
        })
            .WithName("UpdateContract")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractPermissions.Edit);

        app.MapDelete($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteContractCommand(id));
            return Results.Ok("OK");
        })
            .WithName("DeleteContract")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractPermissions.Delete);

        MapStatus(app, baseRoute, "submit-review", ContractStatus.UnderReview, PermissionList.ContractPermissions.SubmitReview);
        MapStatus(app, baseRoute, "sign", ContractStatus.Signed, PermissionList.ContractPermissions.Sign);
        MapStatus(app, baseRoute, "activate", ContractStatus.Active, PermissionList.ContractPermissions.Activate);
        MapStatus(app, baseRoute, "terminate", ContractStatus.Terminated, PermissionList.ContractPermissions.Terminate);

        app.MapPut($"{baseRoute}/{{id:guid}}/renewal-settings", async (Guid id, ConfigureRenewalRequest request, ISender sender) =>
        {
            await sender.Send(new ConfigureContractRenewalCommand(id, request.Settings));
            return Results.Ok("OK");
        })
            .WithName("ConfigureContractRenewal")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractRenewalPermissions.Configure);

        app.MapPost($"{baseRoute}/{{id:guid}}/renew", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ProcessContractRenewalCommand(id));
            return Results.Ok(new { renewal = result.Renewal });
        })
            .WithName("ProcessContractRenewal")
            .Produces<ContractRenewalDto>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractRenewalPermissions.Process);

        app.MapPost($"{baseRoute}/{{id:guid}}/renewals/{{renewalId:guid}}/payment", async (Guid id, Guid renewalId, RecordRenewalPaymentRequest request, ISender sender) =>
        {
            await sender.Send(new RecordContractRenewalPaymentCommand(id, renewalId, request.PaymentReferenceId, request.PaidAmount));
            return Results.Ok("OK");
        })
            .WithName("RecordContractRenewalPayment")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractRenewalPermissions.RecordPayment);
    }

    private static void MapStatus(IEndpointRouteBuilder app, string baseRoute, string route, ContractStatus status, string permission)
    {
        app.MapPost($"{baseRoute}/{{id:guid}}/{route}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new ChangeContractStatusCommand(id, status, route, null));
            return Results.Ok("OK");
        })
            .WithName($"{route}Contract")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(permission);
    }
}
