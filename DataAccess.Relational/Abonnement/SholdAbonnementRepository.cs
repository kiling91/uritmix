using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Relational.Abonnement.Entities;
using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Abonnement;

namespace DataAccess.Relational.Abonnement;

public class SoldAbonnementRepository: ISoldAbonnementRepository
{
    private readonly DbServiceContext _context;
    private readonly IMapper _map;

    public SoldAbonnementRepository(DbServiceContext context, IMapper map)
    {
        _context = context;
        _map = map;
    }

    public Task<SoldAbonnementModel> Create(SoldAbonnementModel model)
    {
        //return CreateEntity(model, context => context.SoldAbonnements);
        throw new NotImplementedException();

    }

    public Task<UpdatedModel<SoldAbonnementModel>> Update(long id, Func<SoldAbonnementModel, Task<SoldAbonnementModel>> updateFunc)
    {
        throw new NotImplementedException();

        /*return await UpdateEntity(
            e => e.Id == id,
            context => context.SoldAbonnements,
            updateFunc,
            (_, _) => Task.CompletedTask);*/
    }

    public Task<SoldAbonnementModel?> Get(long id)
    {
        throw new NotImplementedException();

        /*return await GetEntity(
            e => e.Id == id,
            Dummy<SoldAbonnementModel>,
            context => context.SoldAbonnements);*/
    }

    public Task<PaginatedList<SoldAbonnementModel>> Items(long personId, Paginator paginator)
    {
        throw new NotImplementedException();

        /*var sessions = Context.SoldAbonnements
            .OrderByDescending(p => p.DateSale)
            .Where(p => p.PersonId == personId)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<SoldAbonnementEntity>, PaginatedList<SoldAbonnementModel>>(page);*/
    }
}