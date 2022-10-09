using AutoMapper;
using DataAccess.Auth;
using Helpers.DataAccess.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;

namespace DataAccess.Relational.Auth;

public class RefreshTokenRepository : RepositoryBase<DbServiceContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DbServiceContext context, IMapper mapperObject,
        ILogger<RefreshTokenRepository> logger)
        : base(context, mapperObject, logger)
    {
    }

    public Task<RefreshTokenModel> CreateOrUpdate(RefreshTokenModel model)
    {
        return CreateOrUpdateEntity(
            model,
            context => context.RefreshTokens,
            entity => entity.PersonId == model.PersonId);
    }

    public async Task<RefreshTokenModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<RefreshTokenModel>,
            context => context.RefreshTokens.Include(r => r.Person));
    }
}