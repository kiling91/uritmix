using System.ComponentModel;
using AutoMapper;
using DataAccess.Lesson;
using Dto.Lesson;
using Dto.Person;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Command.Lesson;

public class GetLesson
{
    [DisplayName("GetLessonQuery")]
    public record Query(long LessonId) : IRequest<ResultResponse<LessonView>>;

    public class Handler : IRequestHandler<Query, ResultResponse<LessonView>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly ILessonRepository _lessonRepository;

        public Handler(IStringLocalizer<Handler> localizer, IMapper mapper, ILessonRepository lessonRepository)
        {
            _localizer = localizer;
            _mapper = mapper;
            _lessonRepository = lessonRepository;
        }

        public async Task<ResultResponse<LessonView>> Handle(Query message, CancellationToken ct)
        {
            var lesson = await _lessonRepository.Get(message.LessonId);
            if (lesson == null)
                return ResultResponse<LessonView>.CreateError(_localizer["Lesson not found"]);
            return new ResultResponse<LessonView>(_mapper.Map<LessonView>(lesson));
        }
    }
}