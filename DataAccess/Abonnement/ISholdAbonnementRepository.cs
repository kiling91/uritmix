using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Abonnement;

namespace DataAccess.Abonnement;

public interface ISoldAbonnementRepository
{
    Task<SoldAbonnementModel> Create(SoldAbonnementModel model);

    Task<UpdatedModel<SoldAbonnementModel>> Update(long id,
        Func<SoldAbonnementModel, Task<SoldAbonnementModel>> updateFunc);

    Task<SoldAbonnementModel?> Get(long id);
    Task<PaginatedList<SoldAbonnementModel>> Items(long personId, Paginator paginator);
}