using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using DataAccess.Lesson;
using Dto.Lesson;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Lesson;

namespace Command.Lesson;

public class CreateLesson
{
    [DisplayName("CreateLesson")]
    public record CreateLessonForm
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public long TrainerId { get; init; }
        public int DurationMinute { get; init; }
        public float BasePrice { get; init; }
    }

    [DisplayName("CreateLessonCommand")]
    public record Command(CreateLessonForm Create) : IRequest<ResultResponse<LessonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Create).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Create.Name)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.RoomNameMinLength, ModelSettings.RoomNameMaxLength);

                RuleFor(x => x.Create.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);

                RuleFor(x => x.Create.DurationMinute)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.LessonDurationMinuteMin)
                    .LessThanOrEqualTo(ModelSettings.LessonDurationMinuteMax);

                RuleFor(x => x.Create.BasePrice)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.LessonBasePriceMin)
                    .LessThanOrEqualTo(ModelSettings.LessonDBasePriceMax);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<LessonView>>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer, ILessonRepository lessonRepository,
            IPersonRepository personRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _lessonRepository = lessonRepository;
            _personRepository = personRepository;
        }

        public async Task<ResultResponse<LessonView>> Handle(Command message, CancellationToken ct)
        {
            var create = message.Create with
            {
                Name = message.Create.Name.FirstLetterToUpper(),
                Description = message.Create.Description?.Trim()
            };

            var person = await _personRepository.Get(create.TrainerId);
            if (person == null || !person.IsTrainer)
                return ResultResponse<LessonView>.CreateError(_localizer["Person not found or person is not trainer"]);

            var find = await _lessonRepository.Find(create.Name);
            if (find != null)
                return ResultResponse<LessonView>.CreateError(_localizer["Lesson already exist"]);

            var result = await _lessonRepository.Create(new LessonModel
            {
                Name = create.Name,
                Description = create.Description,
                TrainerId = create.TrainerId,
                DurationMinute = create.DurationMinute,
                BasePrice = create.BasePrice
            });

            return new ResultResponse<LessonView>(_mapper.Map<LessonView>(result));
        }
    }
}