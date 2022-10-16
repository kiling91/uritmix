using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using View.Person;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Command.Person;

public class GetPerson
{
    [DisplayName("GetPersonQuery")]
    public record Query(long PersonId) : IRequest<ResultResponse<PersonView>>;

    public class Handler : IRequestHandler<Query, ResultResponse<PersonView>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IStringLocalizer<Handler> localizer, IMapper mapper, IPersonRepository personRepository)
        {
            _localizer = localizer;
            _mapper = mapper;
            _personRepository = personRepository;
        }

        public async Task<ResultResponse<PersonView>> Handle(Query message, CancellationToken ct)
        {
            var person = await _personRepository.Get(message.PersonId);
            if (person == null)
                return ResultResponse<PersonView>.CreateError(_localizer["Person not found"]);
            return new ResultResponse<PersonView>(_mapper.Map<PersonView>(person));
        }
    }
}