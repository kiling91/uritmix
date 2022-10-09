using System.ComponentModel;
using AutoMapper;
using DataAccess.Auth;
using Dto.Person;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using Helpers.WebApi.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Command.Person;

public class GetPersons
{
    [DisplayName("GetPersonsQuery")]
    public record Query(Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<PersonView>>>;

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

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<PersonView>>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IStringLocalizer<Handler> localizer, IPersonRepository personRepository, IMapper mapper)
        {
            _localizer = localizer;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse<PaginatedListViewModel<PersonView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _personRepository.Items(request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<PersonView>>(items);
            return new ResultResponse<PaginatedListViewModel<PersonView>>(result);
        }
    }
}