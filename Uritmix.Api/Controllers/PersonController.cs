using Command.Auth;
using Command.Person;
using Dto.Person;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("/api/v1/person")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class PersonController : ControllerBase
{
    private readonly IUserAccessor _accessor;
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator, IUserAccessor accessor)
    {
        _mediator = mediator;
        _accessor = accessor;
    }

    /// <summary>
    ///     Возвращает текущего авторизованного пользователя
    /// </summary>
    [HttpGet("self")]
    [AuthorizeByRole]
    public Task<ResultResponse<PersonView>> Self(CancellationToken ct)
    {
        var id = _accessor.UserId();
        return _mediator.Send(new GetPerson.Query(id), ct);
    }

    /// <summary>
    ///     Возвращает пользователя по id
    /// </summary>
    [HttpGet("{personId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<PersonView>> GetPerson(
        long personId,
        CancellationToken ct)
    {
        return _mediator.Send(new GetPerson.Query(personId), ct);
    }

    /// <summary>
    ///     Создает нового пользователя
    /// </summary>
    [HttpPost]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<PersonView>> CreatePerson(
        [FromBody] CreatePerson.CreatePersonForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreatePerson.Command(model), ct);
    }

    /// <summary>
    ///     Обновляет данные пользователя
    /// </summary>
    [HttpPut("{personId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<PersonView>> EditPerson(
        long personId,
        [FromBody] EditPerson.EditPersonForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new EditPerson.Command(personId, model), ct);
    }

    /// <summary>
    ///     Возвращает список пользователей
    /// </summary>
    [HttpGet]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<PersonView>>> GetPersons(
        [FromQuery] PersonTypeView type,
        [FromQuery] Paginator query,
        CancellationToken ct)
    {
        return _mediator.Send(new GetPersons.Query(type, query), ct);
    }
}