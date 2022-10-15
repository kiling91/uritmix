using System.ComponentModel;
using AutoMapper;
using DataAccess.Abonnement;
using Dto.Abonnement;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Command.Abonnement;

public class GetAbonnement
{
    [DisplayName("GetAbonnementQuery")]
    public record Query(long AbonnementId) : IRequest<ResultResponse<AbonnementView>>;

    public class Handler : IRequestHandler<Query, ResultResponse<AbonnementView>>
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;

        public Handler(IStringLocalizer<Handler> localizer, IMapper mapper, IAbonnementRepository abonnementRepository)
        {
            _localizer = localizer;
            _mapper = mapper;
            _abonnementRepository = abonnementRepository;
        }

        public async Task<ResultResponse<AbonnementView>> Handle(Query message, CancellationToken ct)
        {
            var abonnement = await _abonnementRepository.Get(message.AbonnementId);
            if (abonnement == null)
                return ResultResponse<AbonnementView>.CreateError(_localizer["Abonnement not found"]);
            return new ResultResponse<AbonnementView>(_mapper.Map<AbonnementView>(abonnement));
        }
    }
}