using System.Net;

namespace Helpers.Core;

public abstract class RestException : Exception
{
    protected RestException(HttpStatusCode code, string? errors = null)
    {
        Code = code;
        Errors = errors;
    }

    public string? Errors { get; set; }

    public HttpStatusCode Code { get; }
}

public class RestUnprocessableEntityException : RestException
{
    public RestUnprocessableEntityException(string? error = null) : base(HttpStatusCode.UnprocessableEntity, error)
    {
    }
}

public class RestNotFoundException : RestException
{
    public RestNotFoundException(string? error = null) : base(HttpStatusCode.NotFound, error)
    {
    }
}

public class RestArgumentOutOfRangeException : RestException
{
    public RestArgumentOutOfRangeException(string? error = null) : base(HttpStatusCode.InternalServerError, error)
    {
    }
}

public class RestBadRequestException : RestException
{
    public RestBadRequestException(string? error = null) : base(HttpStatusCode.BadRequest, error)
    {
    }
}

public class RestArgumentNullException : RestException
{
    public RestArgumentNullException(string? error = null) : base(HttpStatusCode.InternalServerError, error)
    {
    }
}