using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Auth;
using Model.Person;

namespace DataAccess.Auth;

public interface IPersonRepository
{
    Task<PersonModel> Create(PersonModel model);
    Task<PersonModel?> Get(long id);
    Task<UpdatedModel<PersonModel>> Update(long id, Func<PersonModel, Task<PersonModel>> updateFunc);
    Task<PaginatedList<PersonModel>> Items(PersonType type, Paginator paginator);
    Task<PersonModel?> Find(string email);
    Task<long> RoleCount(AuthRole role);
}