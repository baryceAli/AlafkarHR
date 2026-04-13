namespace AlAfkarERP.Shared.Dtos;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public ErrorResponseDto? Error { get; set; }

    public static ApiResult<T> Success(T data)
        => new() { IsSuccess = true, Data = data };

    public static ApiResult<T> Failure(ErrorResponseDto error)
        => new() { IsSuccess = false, Error = error };
}

public class PaginatedResult<TEntity>
    (int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public IEnumerable<TEntity> Data { get; } = data;
}


public class ErrorResponseDto
{
    public string Title { get; set; } = default!;
    public int Status { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public string Instance { get; set; } = default!;
    public string TraceId { get; set; } = default!;

}
