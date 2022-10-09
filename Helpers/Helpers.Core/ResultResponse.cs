namespace Helpers.Core;

public class ResultResponse<T>
{
    public ResultResponse(T result)
    {
        Result = result;
        Error = null;
        Ok = true;
    }

    public ResultResponse(bool ok, string? error)
    {
        Result = default;
        Error = error;
        Ok = ok;
    }

    public T? Result { get; set; }
    public string? Error { get; set; }
    public bool Ok { get; set; }

    public static ResultResponse<T> CreateError(string error)
    {
        return new ResultResponse<T>(false, error);
    }
}