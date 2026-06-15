using Auth.Contracts.Features.GetByUserName;
using Auth.Users;

namespace Auth.Users.Features.Users.GetByUserName;

public class GetByUserNameHandler(AuthDbContext dbContext) : IQueryHandler<GetByUserNameQuery, GetByUserNameResult>
{
    public async Task<GetByUserNameResult> Handle(GetByUserNameQuery request, CancellationToken cancellationToken)
    {
        var userName = UserNameKeyNormalizer.Normalize(request.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        return new GetByUserNameResult(user.Adapt<UserDto>());
    }
}
