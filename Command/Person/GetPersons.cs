using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using View.Person;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using Helpers.WebApi.Extensions;
using Mapping.Enum.Person;
using MediatR;

namespace Command.Person;

public class GetPersons
{
    [DisplayName("GetPersonsQuery")]
    public record Query
        (PersonTypeView Type, Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<PersonView>>>;

    public class CommandValidator : AbstractValidator<Query>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Paginator).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Paginator).PaginatorValidate();
                RuleFor(x => x.Type)
                    .NotNull();
            });
        }
    }

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<PersonView>>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse<PaginatedListViewModel<PersonView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _personRepository.Items(request.Type.ToModel(), request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<PersonView>>(items);
            return new ResultResponse<PaginatedListViewModel<PersonView>>(result);
        }
    }
}