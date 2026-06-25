namespace Cart.Carts.Features;

public static class CartAuthorization
{
    private const string StoreFrontChannelPrefix = "StoreFront:";

    public static async Task EnsureCartPermissionAsync(
        ClaimsPrincipal? user,
        ISender sender,
        string? channel,
        string cartPermission,
        string storeFrontPermission,
        CancellationToken cancellationToken)
    {
        if (user?.Claims.Any(x => x.Type == "Permission" && x.Value == cartPermission) == true)
            return;

        var storeFrontId = TryParseStoreFrontId(channel);
        if (!storeFrontId.HasValue)
            throw new ForbiddenException($"Missing permission: {cartPermission}");

        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(storeFrontId.Value), cancellationToken);
        await sender.Send(new EnsureCurrentUserBranchPermissionQuery(scope.CompanyId, scope.BranchId, storeFrontPermission), cancellationToken);
    }

    public static async Task<GetStoreFrontBranchScopeResult?> ResolveStoreFrontScopeAsync(
        ISender sender,
        string? channel,
        CancellationToken cancellationToken)
    {
        var storeFrontId = TryParseStoreFrontId(channel);
        return storeFrontId.HasValue
            ? await sender.Send(new GetStoreFrontBranchScopeQuery(storeFrontId.Value), cancellationToken)
            : null;
    }

    public static Guid? TryParseStoreFrontId(string? channel)
    {
        if (string.IsNullOrWhiteSpace(channel) || !channel.StartsWith(StoreFrontChannelPrefix, StringComparison.OrdinalIgnoreCase))
            return null;
        return Guid.TryParse(channel[StoreFrontChannelPrefix.Length..], out var storeFrontId) ? storeFrontId : null;
    }
}
