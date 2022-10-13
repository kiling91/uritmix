using AutoMapper;
using DataAccess.Abonnement;
using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Abonnement;

namespace DataAccess.Relational.Abonnement;

public class AbonnementRepository : IAbonnementRepository
{
    private readonly DbServiceContext _context;
    private readonly IMapper _map;

    public AbonnementRepository(DbServiceContext context, IMapper map)
    {
        _context = context;
        _map = map;
    }

    public Task<AbonnementModel> Create(AbonnementModel model)
    {
        throw new NotImplementedException();

        //return CreateEntity(model, context => context.Abonnements);
    }

    public Task<UpdatedModel<AbonnementModel>> Update(long id, Func<AbonnementModel, Task<AbonnementModel>> updateFunc)
    {
        throw new NotImplementedException();
        /*return await UpdateEntity(
            e => e.Id == id,
            context => context.Abonnements,
            updateFunc,
            (_, _) => Task.CompletedTask);*/
    }

    public Task<AbonnementModel?> Get(long id)
    {
        throw new NotImplementedException();

        /*return await GetEntity(
            e => e.Id == id,
            Dummy<AbonnementModel>,
            context => context.Abonnements.Include(a => a.Lessons));*/
    }

    public Task<AbonnementModel?> Find(string name)
    {
        throw new NotImplementedException();

        /*return await GetEntity(
            e => e.Name == name,
            Dummy<AbonnementModel>,
            context => context.Abonnements);*/
    }

    public Task<PaginatedList<AbonnementModel>> Items(Paginator paginator)
    {
        throw new NotImplementedException();

        /*var sessions = Context.Abonnements
            .Include(a => a.Lessons)
            .OrderBy(p => p.Name)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<AbonnementEntity>, PaginatedList<AbonnementModel>>(page);*/
    }
}