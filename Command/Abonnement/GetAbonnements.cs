using System.ComponentModel;
using AutoMapper;
using DataAccess.Abonnement;
using Dto.Abonnement;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using Helpers.WebApi.Extensions;
using MediatR;

namespace Command.Abonnement;

public class GetAbonnements
{
    [DisplayName("GetAbonnementsQuery")]
    public record Query(Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<AbonnementView>>>;

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

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<AbonnementView>>>
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly IMapper _mapper;

        public Handler(IMapper mapper, IAbonnementRepository abonnementRepository)
        {
            _mapper = mapper;
            _abonnementRepository = abonnementRepository;
        }

        public async Task<ResultResponse<PaginatedListViewModel<AbonnementView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _abonnementRepository.Items(request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<AbonnementView>>(items);
            return new ResultResponse<PaginatedListViewModel<AbonnementView>>(result);
        }
    }
}