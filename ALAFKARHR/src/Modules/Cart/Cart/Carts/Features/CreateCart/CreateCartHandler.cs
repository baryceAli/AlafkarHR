using Cart.Carts.Models;
using Cart.Carts.Features;

namespace Cart.Carts.Features.CreateCart;

public record CreateCartCommand(CartDto Cart) : ICommand<CreateCartResult>;
public record CreateCartResult(Guid Id);

public class CreateCartHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateCartCommand, CreateCartResult>
{
    public async Task<CreateCartResult> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        await CartAuthorization.EnsureCartPermissionAsync(
            httpContextAccessor.HttpContext?.User,
            sender,
            request.Cart.Channel,
            PermissionList.CartPermissions.Create,
            PermissionList.StoreFrontPosPermissions.View,
            cancellationToken);

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var cart = ShoppingCart.Create(request.Cart, userId);
        foreach (var line in request.Cart.Lines)
        {
            cart.AddLine(line, userId);
        }
        await dbContext.Carts.AddAsync(cart, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateCartResult(cart.Id);
    }
}
