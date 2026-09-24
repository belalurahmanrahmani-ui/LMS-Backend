public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
}