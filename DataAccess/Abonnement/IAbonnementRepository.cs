using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Abonnement;

namespace DataAccess.Abonnement;

public interface IAbonnementRepository
{
    Task<AbonnementModel> Create(AbonnementModel model);
    Task<UpdatedModel<AbonnementModel>> Update(long id, Func<AbonnementModel, Task<AbonnementModel>> updateFunc);
    Task<AbonnementModel?> Get(long id);
    Task<AbonnementModel?> Find(string name);
    Task<PaginatedList<AbonnementModel>> Items(Paginator paginator);
}