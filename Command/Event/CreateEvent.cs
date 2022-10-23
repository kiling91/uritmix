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
using Model;
using Model.Event;
using Model.Room;
using View.Event;
using View.Room;

namespace Command.Event;

public class CreateEvent
{
    [DisplayName("CreateEvent")]
    public record CreateEventForm
    {
        public long LessonId { get; init; }
        public long RoomId { get; init; }
        public DateTime StartDate { get; init; }
    }

    [DisplayName("CreateEventCommand")]
    public record Command(CreateEventForm Create) : IRequest<ResultResponse<EventView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Create).NotNull().DependentRules(() =>
            {
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<EventView>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IEventRepository _eventRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILessonRepository _lessonRepository;


        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer, 
            IEventRepository eventRepository, IRoomRepository roomRepository, ILessonRepository lessonRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<ResultResponse<EventView>> Handle(Command message, CancellationToken ct)
        {
            var create = message.Create;

            var t1 = create.StartDate.ToUniversalTime();
            var t2 = DateTime.Now.ToUniversalTime();
            if (t1 < t2)
                return ResultResponse<EventView>.CreateError(_localizer["Start of lesson in the past"]);

            var lesson = await _lessonRepository.Get(create.LessonId);
            if (lesson == null)
                return ResultResponse<EventView>.CreateError(_localizer["Lesson not found"]);

            var room = await _roomRepository.Get(create.RoomId);
            if (room == null)
                return ResultResponse<EventView>.CreateError(_localizer["Room not found"]);

            // TODO: Проверка пересечений занятий в одной комнате
            
            var result = await _eventRepository.Create(new EventModel
            {
                LessonId = create.LessonId,
                RoomId = create.RoomId,
                StartDate = create.StartDate,
                EndDate = create.StartDate.AddMinutes(lesson.DurationMinute)
            });

            return new ResultResponse<EventView>(_mapper.Map<EventView>(result));
        }
    }
}