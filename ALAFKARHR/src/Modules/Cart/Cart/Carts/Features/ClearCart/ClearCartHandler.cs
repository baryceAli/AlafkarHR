namespace Cart.Carts.Features.ClearCart;

public record ClearCartCommand(Guid CartId) : ICommand<ClearCartResult>;
public record ClearCartResult(bool IsSuccess);

public class ClearCartHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ClearCartCommand, ClearCartResult>
{
    public async Task<ClearCartResult> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await dbContext.Carts.Include("_lines").FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken)
            ?? throw new NotFoundException($"Cart not found: {request.CartId}");
        await CartAuthorization.EnsureCartPermissionAsync(
            httpContextAccessor.HttpContext?.User,
            sender,
            cart.Channel,
            PermissionList.CartPermissions.Edit,
            PermissionList.StoreFrontPosPermissions.View,
            cancellationToken);
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        cart.Clear(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ClearCartResult(true);
    }
}
