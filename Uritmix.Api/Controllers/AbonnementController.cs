using Command.Abonnement;
using Dto.Abonnement;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("/api/v1/abonnement")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class AbonnementController : ControllerBase
{
    private readonly IMediator _mediator;

    public AbonnementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Создает новый абонимент
    /// </summary>
    [HttpPost]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<AbonnementView>> CreateAbonnement(
        [FromBody] CreateAbonnement.CreateAbonnementForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreateAbonnement.Command(model), ct);
    }

    /// <summary>
    ///     Обновляет данные абонимента
    /// </summary>
    [HttpPut("{abonnementId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<AbonnementView>> EditAbonnement(
        long abonnementId,
        [FromBody] EditAbonnement.EditAbonnementForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new EditAbonnement.Command(abonnementId, model), ct);
    }

    /// <summary>
    ///     Возвращает список абониментов
    /// </summary>
    [HttpGet]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<AbonnementView>>> GetAbonnements(
        [FromQuery] Paginator query,
        CancellationToken ct)
    {
        return _mediator.Send(new GetAbonnements.Query(query), ct);
    }

    /// <summary>
    ///     Возвращает абоннемент по id
    /// </summary>
    [HttpGet("{abonnementId}")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<AbonnementView>> GetAbonnement(
        long abonnementId,
        CancellationToken ct)
    {
        return _mediator.Send(new GetAbonnement.Query(abonnementId), ct);
    }

    /// <summary>
    ///     Продажа абонимента
    /// </summary>
    [HttpPost("sold")]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<SoldAbonnementView>> SaleAbonnement(
        [FromBody] SaleAbonnement.SaleAbonnementForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new SaleAbonnement.Command(model), ct);
    }

    /// <summary>
    ///     Возвращает список купленных абониментов пользователя
    /// </summary>
    [HttpGet("sold/{personId}")]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<SoldAbonnementView>>> GetSoldAbonnements(
        long personId,
        [FromQuery] Paginator query,
        CancellationToken ct)
    {
        return _mediator.Send(new GetSoldAbonnements.Query(personId, query), ct);
    }
}