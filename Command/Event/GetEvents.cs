using System.ComponentModel;
using AutoMapper;
using DataAccess.Event;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using View.Event;

namespace Command.Event;

public class GetEvents
{
    [DisplayName("GetEventsQuery")]
    public record Query(string FilterExpression) : IRequest<ResultResponse<PaginatedListViewModel<EventView>>>;

    public class CommandValidator : AbstractValidator<Query>
    {
        public CommandValidator()
        {
            RuleFor(x => x.FilterExpression).NotNull().NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<EventView>>>
    {
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;

        public Handler(IMapper mapper, IEventRepository eventRepository)
        {
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        public Task<ResultResponse<PaginatedListViewModel<EventView>>> Handle(Query request,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}