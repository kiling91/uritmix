using System.ComponentModel;
using AutoMapper;
using DataAccess.Lesson;
using View.Lesson;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;

namespace Command.Lesson;

public class EditLesson
{
    [DisplayName("EditLesson")]
    public record EditLessonForm
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public long TrainerId { get; init; }
        public int DurationMinute { get; init; }
        public float BasePrice { get; init; }
    }

    [DisplayName("EditRoomCommand")]
    public record Command(long LessonId, EditLessonForm Edit) : IRequest<ResultResponse<LessonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Edit).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Edit.Name)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.RoomNameMinLength, ModelSettings.RoomNameMaxLength);


                RuleFor(x => x.Edit.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);

                RuleFor(x => x.Edit.DurationMinute)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.LessonDurationMinuteMin)
                    .LessThanOrEqualTo(ModelSettings.LessonDurationMinuteMax);

                RuleFor(x => x.Edit.BasePrice)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.LessonBasePriceMin)
                    .LessThanOrEqualTo(ModelSettings.LessonBasePriceMax);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<LessonView>>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;

        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer, ILessonRepository lessonRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _lessonRepository = lessonRepository;
        }

        public async Task<ResultResponse<LessonView>> Handle(Command message, CancellationToken ct)
        {
            var edit = message.Edit with
            {
                Name = message.Edit.Name.FirstLetterToUpper(),
                Description = message.Edit.Description?.Trim()
            };

            var get = await _lessonRepository.Get(message.LessonId);
            if (get == null)
                return ResultResponse<LessonView>.CreateError(_localizer["Lesson not found"]);

            if (get.Name != edit.Name)
            {
                var find = await _lessonRepository.Find(edit.Name);
                if (find != null)
                    return ResultResponse<LessonView>.CreateError(_localizer["Lesson already exist"]);
            }

            var update = await _lessonRepository.Update(get.Id,
                async model =>
                {
                    var update = model with
                    {
                        Name = edit.Name,
                        Description = edit.Description,
                        TrainerId = edit.TrainerId,
                        DurationMinute = edit.DurationMinute,
                        BasePrice = edit.BasePrice
                    };
                    return await update.AsTaskResult();
                });

            return new ResultResponse<LessonView>(_mapper.Map<LessonView>(update.New));
        }
    }
}