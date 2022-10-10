using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Relational.Abonnement.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.DataAccess.Relational.Extensions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Abonnement;

namespace DataAccess.Relational.Abonnement;

public class AbonnementRepository : RepositoryBase<DbServiceContext>, IAbonnementRepository
{
    public AbonnementRepository(DbServiceContext context, IMapper mapperObject, ILogger<AbonnementRepository> logger)
        : base(context, mapperObject, logger)
    {
    }

    public Task<AbonnementModel> Create(AbonnementModel model)
    {
        return CreateEntity(model, context => context.Abonnements);
    }

    public async Task<UpdatedModel<AbonnementModel>> Update(long id,
        Func<AbonnementModel, Task<AbonnementModel>> updateFunc)
    {
        return await UpdateEntity(
            e => e.Id == id,
            context => context.Abonnements,
            updateFunc,
            (_, _) => Task.CompletedTask);
    }

    public async Task<AbonnementModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<AbonnementModel>,
            context => context.Abonnements.Include(a => a.Lessons));
    }

    public async Task<AbonnementModel?> Find(string name)
    {
        return await GetEntity(
            e => e.Name == name,
            Dummy<AbonnementModel>,
            context => context.Abonnements);
    }

    public async Task<PaginatedList<AbonnementModel>> Items(Paginator paginator)
    {
        var sessions = Context.Abonnements
            .Include(a => a.Lessons)
            .OrderBy(p => p.Name)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<AbonnementEntity>, PaginatedList<AbonnementModel>>(page);
    }
}