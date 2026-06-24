using Cart.Carts.Features;

namespace Cart.Carts.Features.GetCartById;

public record GetCartByIdQuery(Guid Id) : IQuery<GetCartByIdResult>;
public record GetCartByIdResult(CartDto Cart);

public class GetCartByIdHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender) : IQueryHandler<GetCartByIdQuery, GetCartByIdResult>
{
    public async Task<GetCartByIdResult> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        var cart = await dbContext.Carts.Include("_lines").FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Cart not found: {request.Id}");
        await CartAuthorization.EnsureCartPermissionAsync(
            httpContextAccessor.HttpContext?.User,
            sender,
            cart.Channel,
            PermissionList.CartPermissions.View,
            PermissionList.StoreFrontPosPermissions.View,
            cancellationToken);
        return new GetCartByIdResult(cart.ToDto());
    }
}
