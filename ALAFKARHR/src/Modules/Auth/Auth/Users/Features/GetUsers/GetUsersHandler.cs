using Shared.Pagination;

namespace Auth.Users.Features.GetUsers;


public record GetUsersQuery(PaginationRequest PaginationRequest) : IQuery<GetUsersResult>;
public record GetUsersResult(PaginatedResult<UserDto> UserList);
public class GetUsersHandler(AuthDbContext dbContext) : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Users.AsNoTracking().AsQueryable();

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query = query.Where(b =>
                b.UserName.ToLower().Contains(search)
                //|| b.NameEng.ToLower().Contains(search)
            //|| (b.Email != null && b.Email.ToLower().Contains(search)) 
            //|| (b.Phone != null && b.Phone.Contains(search))
            );
        }

        // 📊 Total count AFTER filtering
        long count = await query.LongCountAsync(cancellationToken);

        // 📄 Pagination
        var users = await query
            .OrderBy(b => b.UserName) // default sorting (important!)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetUsersResult(
            new PaginatedResult<UserDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                users.Adapt<List<UserDto>>()
            )
        );

    }
}
