using System.ComponentModel;
using AutoMapper;
using DataAccess.Lesson;
using View.Lesson;
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
        private readonly ILessonRepository _lessonRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;

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