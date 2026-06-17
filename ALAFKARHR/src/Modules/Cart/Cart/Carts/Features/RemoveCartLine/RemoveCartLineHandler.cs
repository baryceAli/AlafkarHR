namespace Cart.Carts.Features.RemoveCartLine;

public record RemoveCartLineCommand(Guid CartId, Guid LineId) : ICommand<RemoveCartLineResult>;
public record RemoveCartLineResult(bool IsSuccess);

public class RemoveCartLineHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveCartLineCommand, RemoveCartLineResult>
{
    public async Task<RemoveCartLineResult> Handle(RemoveCartLineCommand request, CancellationToken cancellationToken)
    {
        var cart = await dbContext.Carts.Include("_lines").FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken)
            ?? throw new NotFoundException($"Cart not found: {request.CartId}");
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        cart.RemoveLine(request.LineId, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveCartLineResult(true);
    }
}
