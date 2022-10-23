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
    public record Query(DateTime StartDate, DateTime EndDate) : IRequest<ResultResponse<IEnumerable<EventView>>>;
 
    public class CommandValidator : AbstractValidator<Query>
    {
        public CommandValidator()
        {
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
        }
    }

    public class Handler : IRequestHandler<Query, ResultResponse<IEnumerable<EventView>>>
    {
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;

        public Handler(IMapper mapper, IEventRepository eventRepository)
        {
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        public async Task<ResultResponse<IEnumerable<EventView>>> Handle(Query request,
            CancellationToken ct)
        {
            
            var items = await _eventRepository.Items(request.StartDate, request.EndDate);
            var result = _mapper.Map<IEnumerable<EventView>>(items);
            return new ResultResponse<IEnumerable<EventView>>(result);
        }
    }
}