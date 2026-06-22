namespace SalesOrder.Orders.Features.CreateManualSalesOrder;

using SalesOrder.Orders.Features.CreateOrder;

public record CreateManualSalesOrderCommand(CreateManualSalesOrderDto SalesOrder) : ICommand<CreateManualSalesOrderResult>;
public record CreateManualSalesOrderResult(Guid Id, string Number);
public record CreateManualSalesOrderRequest(CreateManualSalesOrderDto SalesOrder);
public record CreateManualSalesOrderResponse(Guid Id, string Number);

public class CreateManualSalesOrderHandler(ISender sender)
    : ICommandHandler<CreateManualSalesOrderCommand, CreateManualSalesOrderResult>
{
    public async Task<CreateManualSalesOrderResult> Handle(CreateManualSalesOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.SalesOrder.CustomerId == Guid.Empty)
            throw new Exception("Customer is required.");

        if (request.SalesOrder.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");

        if (request.SalesOrder.Lines.Count == 0)
            throw new Exception("Sales order must include at least one line.");

        var total = request.SalesOrder.Lines.Sum(x => x.Quantity * x.UnitPrice);
        var eligibility = await sender.Send(
            new GetCustomerSalesEligibilityQuery(request.SalesOrder.CustomerId, request.SalesOrder.CompanyId, total),
            cancellationToken);

        if (!eligibility.Exists || !eligibility.IsActive)
            throw new Exception(eligibility.BlockReason ?? "Customer is not eligible for sales.");

        request.SalesOrder.SourceType = SalesOrderSourceType.Manual;
        request.SalesOrder.SourceDocumentId = null;
        request.SalesOrder.SourceDocumentNumber = null;

        var created = await sender.Send(new CreateOrderCommand(request.SalesOrder), cancellationToken);
        return new CreateManualSalesOrderResult(created.Id, request.SalesOrder.Number);
    }
}

public class CreateManualSalesOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/SalesOrders/manual", async (CreateManualSalesOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateManualSalesOrderCommand>());
            return Results.Created($"/api/v1/SalesOrders/Order/{result.Id}", result.Adapt<CreateManualSalesOrderResponse>());
        })
            .WithName("CreateManualSalesOrder")
            .Produces<CreateManualSalesOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create manual sales order")
            .WithDescription("Creates a draft sales order for back-office sales users.")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Create);
    }
}
