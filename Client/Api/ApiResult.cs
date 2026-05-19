namespace Client.Api;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }

    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RawBody { get; set; }
    public string? ExceptionType { get; set; }
}