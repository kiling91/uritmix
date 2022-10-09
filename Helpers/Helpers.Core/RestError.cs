namespace Helpers.Core;

public class ErrorResponse
{
    public ErrorResponse(string code, string? error = null)
    {
        Error = error ?? "Unknown error";
        Code = code;
    }

    public string Error { get; init; }
    public string Code { get; init; }
}