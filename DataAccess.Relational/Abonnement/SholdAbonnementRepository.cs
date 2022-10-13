using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Relational.Abonnement.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.Extensions.Logging;
using Model.Abonnement;

namespace DataAccess.Relational.Abonnement;

public class SoldAbonnementRepository : RepositoryBase<DbServiceContext>, ISoldAbonnementRepository
{
    public SoldAbonnementRepository(DbServiceContext context, IMapper map, ILogger<SoldAbonnementRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<SoldAbonnementModel> Create(SoldAbonnementModel model)
    {
        return CreateEntity(model, c => c.SoldAbonnements);
    }

    public Task<UpdatedModel<SoldAbonnementModel>> Update(long id,
        Func<SoldAbonnementModel, Task<SoldAbonnementModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.SoldAbonnements, updateFunc);
    }

    public Task<SoldAbonnementModel?> Get(long id)
    {
        return GetEntity<SoldAbonnementModel, SoldAbonnementEntity>(
            e => e.Id == id,
            c => c.SoldAbonnements);
    }

    public Task<PaginatedList<SoldAbonnementModel>> Items(long personId, Paginator paginator)
    {
        var query = Context.SoldAbonnements
            .OrderByDescending(p => p.DateSale)
            .Where(p => p.PersonId == personId);
        return PaginatedEntity<SoldAbonnementModel, SoldAbonnementEntity>(paginator, query);
    }
}