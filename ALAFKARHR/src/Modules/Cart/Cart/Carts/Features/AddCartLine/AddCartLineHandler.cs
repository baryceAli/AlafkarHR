using Cart.Carts.Features;

namespace Cart.Carts.Features.AddCartLine;

public record AddCartLineCommand(Guid CartId, CartLineDto Line) : ICommand<AddCartLineResult>;
public record AddCartLineResult(bool IsSuccess);

public class AddCartLineHandler(CartDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<AddCartLineCommand, AddCartLineResult>
{
    public async Task<AddCartLineResult> Handle(AddCartLineCommand request, CancellationToken cancellationToken)
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
        cart.AddLine(request.Line, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AddCartLineResult(true);
    }
}
