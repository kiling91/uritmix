using AutoMapper;
using DataAccess.Protocol;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.Extensions.Logging;
using Model.Protocol;

namespace DataAccess.Relational.Protocol;

public class ProtocolRepository: RepositoryBase<DbServiceContext>, IProtocolRepository
{
    public ProtocolRepository(DbServiceContext context, IMapper map, ILogger<ProtocolRepository> logger) : base(context, map, logger)
    {
    }

    public Task<ProtocolModel> Create(ProtocolModel model)
    {
        throw new NotImplementedException();
    }

    public Task<UpdatedModel<ProtocolModel>> Update(long id, Func<ProtocolModel, Task<ProtocolModel>> updateFunc)
    {
        throw new NotImplementedException();
    }

    public Task<ProtocolModel?> Get(long id)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<ProtocolModel>> Items(Paginator paginator)
    {
        throw new NotImplementedException();
    }
}