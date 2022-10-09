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

public class SoldAbonnementRepository: RepositoryBase<DbServiceContext>, ISoldAbonnementRepository
{
    public SoldAbonnementRepository(DbServiceContext context, IMapper mapperObject, ILogger<SoldAbonnementRepository> logger) 
        : base(context, mapperObject, logger)
    {
    }

    public Task<SoldAbonnementModel> Create(SoldAbonnementModel model)
    {
        return CreateEntity(model, context => context.SoldAbonnements);
    }

    public async Task<UpdatedModel<SoldAbonnementModel>> Update(long id, Func<SoldAbonnementModel, Task<SoldAbonnementModel>> updateFunc)
    {
        return await UpdateEntity(
            e => e.Id == id,
            context => context.SoldAbonnements,
            updateFunc,
            (_, _) => Task.CompletedTask);
    }

    public async Task<SoldAbonnementModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<SoldAbonnementModel>,
            context => context.SoldAbonnements);
    }

    public async Task<PaginatedList<SoldAbonnementModel>> Items(long personId, Paginator paginator)
    {
        var sessions = Context.SoldAbonnements
            .OrderByDescending(p => p.DateSale)
            .Where(p => p.PersonId == personId)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<SoldAbonnementEntity>, PaginatedList<SoldAbonnementModel>>(page);
    }
}