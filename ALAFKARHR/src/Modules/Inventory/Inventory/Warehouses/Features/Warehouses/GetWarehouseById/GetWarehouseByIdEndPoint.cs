
namespace Inventory.Warehouses.Features.Warehouses.GetWarehouseById;

//public record GetWarehouseByIdRequest(Guid Id);

public record GetWarehouseByIdResponse(WarehouseDto Warehouse);
public class GetWarehouseByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/warehouses/{id:guid}", async (Guid id,[FromServices] ISender sender) =>
        {
            var query=new GetWarehouseByIdQuery(id);
            var result=await sender.Send(query);
            var response=new GetWarehouseByIdResponse(result.Warehouse);
            return Results.Ok(response);
        })
            .WithName("GetWarehouseById")
            .Produces<GetWarehouseByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Warehouse By Id")
            .WithDescription("Get Warehouse By Id");
    }
}
