using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Relational.Abonnement.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Abonnement;

namespace DataAccess.Relational.Abonnement;

public class AbonnementRepository : RepositoryBase<DbServiceContext>, IAbonnementRepository
{
    public AbonnementRepository(DbServiceContext context, IMapper map, ILogger<AbonnementRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<AbonnementModel> Create(AbonnementModel model)
    {
        return CreateEntity(model, c => c.Abonnements);
    }

    public Task<UpdatedModel<AbonnementModel>> Update(long id, Func<AbonnementModel, Task<AbonnementModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.Abonnements, updateFunc);
    }

    public Task<AbonnementModel?> Get(long id)
    {
        return GetEntity<AbonnementModel, AbonnementEntity>(
            e => e.Id == id,
            c => c.Abonnements.Include(l => l.Lessons));
    }

    public Task<AbonnementModel?> Find(string name)
    {
        return GetEntity<AbonnementModel, AbonnementEntity>(
            e => e.Name == name, 
            c => c.Abonnements.Include(l => l.Lessons));
    }

    public Task<PaginatedList<AbonnementModel>> Items(Paginator paginator)
    {
        var query = Context.Abonnements
            .Include(l => l.Lessons)
            .OrderBy(p => p.Name);
        return PaginatedEntity<AbonnementModel, AbonnementEntity>(paginator, query);
    }
}