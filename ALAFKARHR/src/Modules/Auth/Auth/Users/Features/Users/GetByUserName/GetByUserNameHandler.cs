using Auth.Contracts.Features.GetByUserName;
using Shared.Exceptions;

namespace Auth.Users.Features.Users.GetByUserName;

public class GetByUserNameHandler(AuthDbContext dbContext) : IQueryHandler<GetByUserNameQuery, GetByUserNameResult>
{
    public async Task<GetByUserNameResult> Handle(GetByUserNameQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User not found: {request.UserName}");

        return new GetByUserNameResult(user.Adapt<UserDto>());
    }
}
