using System.ComponentModel;
using AutoMapper;
using DataAccess.Event;
using DataAccess.Lesson;
using DataAccess.Room;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model.Event;
using View.Event;
using View.Lesson;

namespace Command.Event;

public class DeleteEvent
{
    [DisplayName("DeleteEventCommand")]
    public record Command(long EventId) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<Unit>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IEventRepository _eventRepository;


        public Handler(IStringLocalizer<Handler> localizer, IEventRepository eventRepository)
        {
            _localizer = localizer;
            _eventRepository = eventRepository;
        }

        public async Task<ResultResponse<Unit>> Handle(Command message, CancellationToken ct)
        {
            var get = await _eventRepository.Get(message.EventId);
            if (get == null)
                return ResultResponse<Unit>.CreateError(_localizer["Event not found"]);
            if (get.Type == EventType.InProgress)
                return ResultResponse<Unit>.CreateError(_localizer["Event in progress"]);
            if (get.Type == EventType.Finished)
                return ResultResponse<Unit>.CreateError(_localizer["Event is finished"]);

            await _eventRepository.Remove(message.EventId);
            
            return new ResultResponse<Unit>(Unit.Value);
        }
    }
}