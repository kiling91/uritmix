using AutoMapper;
using DataAccess.Auth;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;
using Model.Person;

namespace DataAccess.Relational.Auth;

public class PersonRepository : RepositoryBase<DbServiceContext>, IPersonRepository
{
    public PersonRepository(DbServiceContext context, IMapper map, ILogger<PersonRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<PersonModel> Create(PersonModel model)
    {
        return CreateEntity(model, c => c.Persons);
    }

    public Task<PersonModel?> Get(long id)
    {
        return GetEntity<PersonModel, PersonEntity>(e => e.Id == id, c => c.Persons);
    }

    public Task<UpdatedModel<PersonModel>> Update(long id, Func<PersonModel, Task<PersonModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.Persons, updateFunc);
    }

    public Task<PaginatedList<PersonModel>> Items(PersonType type, Paginator paginator)
    {
        var query = Context.Persons
            .Include(p => p.Auth)
            .OrderBy(p => p.Id)
            .AsQueryable();

        if (type == PersonType.Trainer)
            query = query.Where(p => p.IsTrainer);
        else if (type == PersonType.Account)
            query = query.Where(p => p.HaveAuth);

        return PaginatedEntity<PersonModel, PersonEntity>(paginator, query);
    }

    public Task<PersonModel?> Find(string email)
    {
        return GetEntity<PersonModel, PersonEntity>(e => e.Auth != null && e.Auth.Email == email,
            c => c.Persons.Include(p => p.Auth));
    }

    public Task<long> RoleCount(AuthRole role)
    {
        return Context.Auth.Where(u => u.Role == (byte)role).LongCountAsync();
    }
}