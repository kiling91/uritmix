using Command.Lesson;
using Dto.Lesson;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("/api/v1/lesson")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class LessonController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Создает новое занятие
    /// </summary>
    [HttpPost]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<LessonView>> CreateLesson(
        [FromBody] CreateLesson.CreateLessonForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreateLesson.Command(model), ct);
    }

    /// <summary>
    ///     Обновляет данные занятия
    /// </summary>
    [HttpPut("{lessonId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<LessonView>> EditRoom(
        long lessonId,
        [FromBody] EditLesson.EditLessonForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new EditLesson.Command(lessonId, model), ct);
    }

    /// <summary>
    ///     Возвращает занятие по id
    /// </summary>
    [HttpGet("{lessonId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<LessonView>> GetLesson(
        long lessonId,
        CancellationToken ct)
    {
        return _mediator.Send(new GetLesson.Query(lessonId), ct);
    }

    /// <summary>
    ///     Возвращает список занятий
    /// </summary>
    [HttpGet]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<LessonView>>> GetRooms(
        [FromQuery] Paginator query,
        CancellationToken ct)
    {
        return _mediator.Send(new GetLessons.Query(query), ct);
    }
}