using Auth.Contracts.Features.UserCompanyMembership;

namespace Auth.Users.Features.Users.UserCompanyMembership;

public class UserBelongsToCompanyHandler(AuthDbContext dbContext)
    : IQueryHandler<UserBelongsToCompanyQuery, UserBelongsToCompanyResult>
{
    public async Task<UserBelongsToCompanyResult> Handle(UserBelongsToCompanyQuery request, CancellationToken cancellationToken)
    {
        var belongs = await dbContext.Users.AsNoTracking()
            .AnyAsync(x => x.Id == request.UserId && x.CompanyId == request.CompanyId, cancellationToken);

        return new UserBelongsToCompanyResult(belongs);
    }
}
