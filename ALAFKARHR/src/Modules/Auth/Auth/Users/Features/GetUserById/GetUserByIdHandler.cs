using Auth.Contracts.Dtos;
using Auth.Contracts.Features.GetUserById;
using Shared.Exceptions;

namespace Auth.Users.Features.GetUserById;


public class GetUserByIdHandler(AuthDbContext dbContext) : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user=await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id==request.Id,cancellationToken);
        if (user is null)
            throw new NotFoundException($"Uesr not found: {request.Id}");

        return new GetUserByIdResult(user.Adapt<UserDto>());
    }
}
