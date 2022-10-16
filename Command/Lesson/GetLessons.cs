using System.ComponentModel;
using AutoMapper;
using DataAccess.Lesson;
using View.Lesson;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using Helpers.WebApi.Extensions;
using MediatR;

namespace Command.Lesson;

public class GetLessons
{
    [DisplayName("GetLessonsQuery")]
    public record Query(Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<LessonView>>>;

    public class CommandValidator : AbstractValidator<Query>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Paginator).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Paginator).PaginatorValidate();
            });
        }
    }

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<LessonView>>>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IMapper _mapper;

        public Handler(IMapper mapper, ILessonRepository lessonRepository)
        {
            _mapper = mapper;
            _lessonRepository = lessonRepository;
        }

        public async Task<ResultResponse<PaginatedListViewModel<LessonView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _lessonRepository.Items(request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<LessonView>>(items);
            return new ResultResponse<PaginatedListViewModel<LessonView>>(result);
        }
    }
}