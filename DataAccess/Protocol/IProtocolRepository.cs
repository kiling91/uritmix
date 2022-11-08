using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Protocol;

namespace DataAccess.Protocol;

public interface IProtocolRepository
{
    Task<ProtocolModel> Create(ProtocolModel model);
    Task<UpdatedModel<ProtocolModel>> Update(long id, Func<ProtocolModel, Task<ProtocolModel>> updateFunc);
    Task<ProtocolModel?> Get(long id);
    Task<PaginatedList<ProtocolModel>> Items(Paginator paginator);
}