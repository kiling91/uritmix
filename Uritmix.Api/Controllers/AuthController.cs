using Command.Auth;
using Helpers.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;
using View.Auth;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status200OK)]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Авторизация пользователя
    /// </summary>
    [HttpPost("login")]
    public Task<ResultResponse<LoggedPersonView>> Login([FromBody] Login.LoginUserForm model, CancellationToken ct)
    {
        return _mediator.Send(new Login.Command(model), ct);
    }

    /// <summary>
    ///     Разлогин пользователя
    /// </summary>
    [HttpPost("logout")]
    public Task<ResultResponse<Unit>> Login([FromBody] Logout.LogoutForm model, CancellationToken ct)
    {
        return _mediator.Send(new Logout.Command(model), ct);
    }

    /// <summary>
    ///     Получение нового token с помощью refresh token
    /// </summary>
    [HttpPost("refresh")]
    public Task<ResultResponse<LoggedPersonView>> Refresh([FromBody] Refresh.RefreshForm model, CancellationToken ct)
    {
        return _mediator.Send(new Refresh.Command(model), ct);
    }

    /// <summary>
    ///     Запрос на смену пароля
    /// </summary>
    [HttpPost("password-reset-query")]
    public Task<ResultResponse<Unit>> PasswordResetQuery(
        [FromBody] PasswordResetQuery.PasswordResetQueryForm passwordReset, CancellationToken ct)
    {
        return _mediator.Send(new PasswordResetQuery.Command(passwordReset), ct);
    }

    /// <summary>
    ///     Смена пароля на основе токена отправленного на почту
    /// </summary>
    [HttpPost("password-reset")]
    public Task<ResultResponse<Unit>> PasswordReset(
        [FromBody] PasswordReset.PasswordResetForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new PasswordReset.Command(model), ct);
    }

    /// <summary>
    ///     Активация созданного пользователя
    /// </summary>
    [HttpPost("activate")]
    public Task<ResultResponse<Unit>> PasswordReset(
        [FromBody] ActivateAuth.ActivateAuthForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new ActivateAuth.Command(model), ct);
    }

    /// <summary>
    ///     Создание аккаунта для пользователя
    /// </summary>
    [HttpPost("{personId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<Unit>> CreateAuth(
        long personId,
        [FromBody] CreateAuth.CreateAuthForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreateAuth.Command(personId, model), ct);
    }
}