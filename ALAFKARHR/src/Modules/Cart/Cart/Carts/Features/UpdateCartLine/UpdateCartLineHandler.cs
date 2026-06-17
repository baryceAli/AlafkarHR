namespace Cart.Carts.Features.UpdateCartLine;

public record UpdateCartLineCommand(Guid CartId, Guid LineId, decimal Quantity) : ICommand<UpdateCartLineResult>;
public record UpdateCartLineResult(bool IsSuccess);

public class UpdateCartLineHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCartLineCommand, UpdateCartLineResult>
{
    public async Task<UpdateCartLineResult> Handle(UpdateCartLineCommand request, CancellationToken cancellationToken)
    {
        var cart = await dbContext.Carts.Include("_lines").FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken)
            ?? throw new NotFoundException($"Cart not found: {request.CartId}");
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        cart.UpdateLineQuantity(request.LineId, request.Quantity, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateCartLineResult(true);
    }
}
