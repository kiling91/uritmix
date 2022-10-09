using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using Dto.Person;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Model;
using Model.Person;

namespace Command.Person;

public class CreatePerson
{
    [DisplayName("CreatePerson")]
    public record CreatePersonForm
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public bool IsTrainer { get; init; }
        public string? Description { get; init; } = null!;
    }

    [DisplayName("CreatePersonCommand")]
    public record Command(CreatePersonForm Create) : IRequest<ResultResponse<PersonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Create).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Create.FirstName)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.Create.LastName)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.Create.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<PersonView>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse<PersonView>> Handle(Command message, CancellationToken ct)
        {
            var create = message.Create with
            {
                FirstName = message.Create.FirstName.FirstLetterToUpper(),
                LastName = message.Create.LastName.FirstLetterToUpper()
            };

            var result = await _personRepository.Create(new PersonModel
            {
                FirstName = create.FirstName,
                LastName = create.LastName,
                IsTrainer = create.IsTrainer,
                Description = create.Description,
                HaveAuth = false
            });

            return new ResultResponse<PersonView>(_mapper.Map<PersonView>(result));
        }
    }
}