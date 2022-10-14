using System.ComponentModel;
using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Lesson;
using Dto.Abonnement;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using Mapping.Enum.Abonnement;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Lesson;

namespace Command.Abonnement;

public class EditAbonnement
{
    [DisplayName("EditAbonnement")]
    public record EditAbonnementForm
    {
        public string Name { get; init; } = null!;
        public AbonnementValidityView Validity { get; init; }
        public byte NumberOfVisits { get; init; }
        public float BasePrice { get; init; }

        public DiscountView Discount { get; init; }

        // public byte DaysOfFreezing { get; init; }
        public IEnumerable<long> LessonIds { get; init; } = null!;
    }

    [DisplayName("EditAbonnementCommand")]
    public record Command(long AbonnementId, EditAbonnementForm Edit) : IRequest<ResultResponse<AbonnementView>>;

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

                RuleFor(x => x.Edit.NumberOfVisits)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.AbonnementNumberOfVisitsMin)
                    .LessThanOrEqualTo(ModelSettings.AbonnementNumberOfVisitsMax);

                //RuleFor(x => x.Edit.DaysOfFreezing)
                //    .NotNull()
                //    .GreaterThanOrEqualTo(ModelSettings.AbonnementDaysOfFreezingMin)
                //    .LessThanOrEqualTo(ModelSettings.AbonnementDaysOfFreezingMax);

                RuleFor(x => x.Edit.BasePrice)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.AbonnementBasePriceMin)
                    .LessThanOrEqualTo(ModelSettings.AbonnementBasePriceMax);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<AbonnementView>>
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;

        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer,
            IAbonnementRepository abonnementRepository, ILessonRepository lessonRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _abonnementRepository = abonnementRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<ResultResponse<AbonnementView>> Handle(Command message, CancellationToken ct)
        {
            var edit = message.Edit with
            {
                Name = message.Edit.Name.FirstLetterToUpper()
            };

            var get = await _abonnementRepository.Get(message.AbonnementId);
            if (get == null)
                return ResultResponse<AbonnementView>.CreateError(_localizer["Abonnement not found"]);

            if (get.Name != edit.Name)
            {
                var find = await _abonnementRepository.Find(edit.Name);
                if (find != null)
                    return ResultResponse<AbonnementView>.CreateError(_localizer["Abonnement already exist"]);
            }

            var lessons = await _lessonRepository.Find(edit.LessonIds.ToArray());
            if (lessons.Count != edit.LessonIds.Count())
                return ResultResponse<AbonnementView>.CreateError(_localizer["Lesson not found"]);

            var update = await _abonnementRepository.Update(get.Id,
                async model =>
                {
                    var update = model with
                    {
                        Name = edit.Name,
                        Validity = edit.Validity.ToModel(),
                        NumberOfVisits = edit.NumberOfVisits,
                        BasePrice = edit.BasePrice,
                        Discount = edit.Discount.ToModel(),
                        // DaysOfFreezing = edit.DaysOfFreezing,
                        Lessons = lessons
                    };
                    return await update.AsTaskResult();
                });

            return new ResultResponse<AbonnementView>(_mapper.Map<AbonnementView>(update.New));
        }
    }
}