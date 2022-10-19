using Command.Room;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;
using View.Room;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("/api/v1/room")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class RoomController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Создает новое помещение
    /// </summary>
    [HttpPost]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<RoomView>> CreateRoom(
        [FromBody] CreateRoom.CreateRoomForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreateRoom.Command(model), ct);
    }

    /// <summary>
    ///     Обновляет данные помещения
    /// </summary>
    [HttpPut("{roomId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<RoomView>> EditRoom(
        long roomId,
        [FromBody] EditRoom.EditRoomForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new EditRoom.Command(roomId, model), ct);
    }
    
    /// <summary>
    ///     Возвращает помещение по id
    /// </summary>
    [HttpGet("{roomId}")]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<RoomView>> GetRoom(
        long roomId,
        CancellationToken ct)
    {
        return _mediator.Send(new GetRoom.Query(roomId), ct);
    }

    /// <summary>
    ///     Возвращает список помещений
    /// </summary>
    [HttpGet]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<RoomView>>> GetRooms(
        [FromQuery] Paginator query,
        CancellationToken ct)
    {
        return _mediator.Send(new GetRooms.Query(query), ct);
    }
}