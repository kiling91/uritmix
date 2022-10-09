using AutoMapper;
using DataAccess.Auth;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.DataAccess.Relational.Extensions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;
using Model.Person;

namespace DataAccess.Relational.Auth;

public class PersonRepository : RepositoryBase<DbServiceContext>, IPersonRepository
{
    public PersonRepository(DbServiceContext context, IMapper mapperObject, ILogger<PersonRepository> logger)
        : base(context, mapperObject, logger)
    {
    }

    public Task<PersonModel> Create(PersonModel user)
    {
        return CreateEntity(user, context => context.Persons);
    }

    public async Task<PersonModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<PersonModel>,
            context => context.Persons.Include(p => p.Auth));
    }

    public async Task<UpdatedModel<PersonModel>> Update(long id, Func<PersonModel, Task<PersonModel>> updateFunc)
    {
        return await UpdateEntity(
            e => e.Id == id,
            context => context.Persons.Include(p => p.Auth),
            updateFunc,
            (context, entity) => Task.CompletedTask);
    }

    public async Task<PaginatedList<PersonModel>> Items(PersonType type, Paginator paginator)
    {
        var sessions = Context.Persons
            .Include(p => p.Auth)
            .OrderBy(p => p.Id).AsQueryable();

        if (type == PersonType.Trainer)
            sessions = sessions.Where(p => p.IsTrainer);
        else if (type == PersonType.Account)
            sessions = sessions.Where(p => p.HaveAuth);

        sessions = sessions.AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<PersonEntity>, PaginatedList<PersonModel>>(page);
    }

    public async Task<PersonModel?> Find(string email)
    {
        return await GetEntity(
            e => e.Auth != null && e.Auth.Email == email,
            Dummy<PersonModel>,
            context => context.Persons.Include(p => p.Auth));
    }

    public Task<long> RoleCount(AuthRole role)
    {
        return Context.Auth.Where(u => u.Role == (byte)role).LongCountAsync();
    }
}