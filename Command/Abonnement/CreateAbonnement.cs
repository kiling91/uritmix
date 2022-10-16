using System.ComponentModel;
using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Lesson;
using View.Abonnement;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using Mapping.Enum.Abonnement;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Abonnement;

namespace Command.Abonnement;

public class CreateAbonnement
{
    [DisplayName("CreateAbonnement")]
    public record CreateAbonnementForm
    {
        public string Name { get; init; } = null!;
        public AbonnementValidityView Validity { get; init; }
        public byte MaxNumberOfVisits { get; init; }
        public float BasePrice { get; init; }

        public DiscountView MaxDiscount { get; init; }

        // public byte DaysOfFreezing { get; init; }
        public IEnumerable<long> LessonIds { get; init; } = null!;
    }

    [DisplayName("CreateAbonnementCommand")]
    public record Command(CreateAbonnementForm Create) : IRequest<ResultResponse<AbonnementView>>;

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

                RuleFor(x => x.Create.MaxNumberOfVisits)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.AbonnementNumberOfVisitsMin)
                    .LessThanOrEqualTo(ModelSettings.AbonnementNumberOfVisitsMax);

                /*RuleFor(x => x.Create.DaysOfFreezing)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.AbonnementDaysOfFreezingMin)
                    .LessThanOrEqualTo(ModelSettings.AbonnementDaysOfFreezingMax);*/

                RuleFor(x => x.Create.BasePrice)
                    .NotNull()
                    .GreaterThanOrEqualTo(ModelSettings.AbonnementBasePriceMin)
                    .LessThanOrEqualTo(ModelSettings.AbonnementBasePriceMax);

                RuleFor(x => x.Create.LessonIds)
                    .NotNull();
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
            var create = message.Create with
            {
                Name = message.Create.Name.FirstLetterToUpper()
            };

            if (!create.LessonIds.Any())
                return ResultResponse<AbonnementView>.CreateError(_localizer["Abonnement must be assigned lessens"]);

            var find = await _abonnementRepository.Find(create.Name);
            if (find != null)
                return ResultResponse<AbonnementView>.CreateError(_localizer["Abonnement already exist"]);

            var lessons = await _lessonRepository.Find(create.LessonIds.ToArray());
            if (lessons.Count != create.LessonIds.Count())
                return ResultResponse<AbonnementView>.CreateError(_localizer["Lesson not found"]);

            var result = await _abonnementRepository.Create(new AbonnementModel
            {
                Name = create.Name,
                Validity = create.Validity.ToModel(),
                MaxNumberOfVisits = create.MaxNumberOfVisits,
                BasePrice = create.BasePrice,
                MaxDiscount = create.MaxDiscount.ToModel(),
                // DaysOfFreezing = create.DaysOfFreezing,
                Lessons = lessons
            });

            return new ResultResponse<AbonnementView>(_mapper.Map<AbonnementView>(result));
        }
    }
}