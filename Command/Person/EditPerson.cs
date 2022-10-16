using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using View.Person;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;

namespace Command.Person;

public class EditPerson
{
    [DisplayName("EditPerson")]
    public record EditPersonForm
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Description { get; init; } = null!;
    }

    [DisplayName("EditPersonCommand")]
    public record Command(long PersonId, EditPersonForm Edit) : IRequest<ResultResponse<PersonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Edit).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Edit.FirstName)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.Edit.LastName)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.Edit.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<PersonView>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, IMapper mapper, IStringLocalizer<Handler> localizer)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ResultResponse<PersonView>> Handle(Command message, CancellationToken ct)
        {
            var edit = message.Edit with
            {
                FirstName = message.Edit.FirstName.FirstLetterToUpper(),
                LastName = message.Edit.LastName.FirstLetterToUpper()
            };

            var find = await _personRepository.Get(message.PersonId);
            if (find == null)
                return ResultResponse<PersonView>.CreateError(_localizer["Person not found"]);

            var update = await _personRepository.Update(find.Id,
                async model =>
                {
                    var update = model with
                    {
                        FirstName = edit.FirstName,
                        LastName = edit.LastName,
                        Description = edit.Description
                    };
                    return await update.AsTaskResult();
                });

            return new ResultResponse<PersonView>(_mapper.Map<PersonView>(update.New));
        }
    }
}