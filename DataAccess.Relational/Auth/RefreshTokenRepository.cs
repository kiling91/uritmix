using AutoMapper;
using DataAccess.Auth;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;

namespace DataAccess.Relational.Auth;

public class RefreshTokenRepository : RepositoryBase<DbServiceContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DbServiceContext context, IMapper map,
        ILogger<RefreshTokenRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<RefreshTokenModel> CreateOrUpdate(RefreshTokenModel model)
    {
        return CreateOrUpdateEntity(
            model,
            context => context.RefreshTokens,
            entity => entity.PersonId == model.PersonId);
    }

    public Task<RefreshTokenModel?> Get(long id)
    {
        return GetEntity<RefreshTokenModel, RefreshTokenEntity>(
            e => e.Id == id,
            c => c.RefreshTokens
                .Include(code => code.Person));
    }
}