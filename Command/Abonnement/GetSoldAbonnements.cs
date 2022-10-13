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

public class GetSoldAbonnements
{
    [DisplayName("GetSoldAbonnementsQuery")]
    public record Query
        (long PersonId, Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<SoldAbonnementView>>>;

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

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<SoldAbonnementView>>>
    {
        private readonly IMapper _mapper;
        private readonly ISoldAbonnementRepository _soldAbonnementRepository;

        public Handler(IMapper mapper, ISoldAbonnementRepository soldAbonnementRepository)
        {
            _mapper = mapper;
            _soldAbonnementRepository = soldAbonnementRepository;
        }

        public async Task<ResultResponse<PaginatedListViewModel<SoldAbonnementView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _soldAbonnementRepository.Items(request.PersonId, request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<SoldAbonnementView>>(items);
            return new ResultResponse<PaginatedListViewModel<SoldAbonnementView>>(result);
        }
    }
}