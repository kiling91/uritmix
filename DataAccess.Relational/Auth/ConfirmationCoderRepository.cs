using AutoMapper;
using DataAccess.Auth;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;

namespace DataAccess.Relational.Auth;

public class ConfirmationCoderRepository : RepositoryBase<DbServiceContext>, IConfirmationCoderRepository
{
    public ConfirmationCoderRepository(DbServiceContext context, IMapper map,
        ILogger<ConfirmationCoderRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<ConfirmationCodeModel> Create(ConfirmationCodeModel model)
    {
        return CreateEntity(model, c => c.ConfirmationCodes);
    }

    public async Task Remove(string token)
    {
        await DeleteEntity<ConfirmationCodeModel, ConfirmationCodeEntity>(
            e => e.Token == token,
            context => context.ConfirmationCodes);
    }

    public async Task Remove(long personId, ConfirmTokenType type)
    {
        await DeleteEntity<ConfirmationCodeModel, ConfirmationCodeEntity>(
            e => e.PersonId == personId && e.Type == (byte)type,
            context => context.ConfirmationCodes);
    }

    public Task<ConfirmationCodeModel?> Find(string token)
    {
        return GetEntity<ConfirmationCodeModel, ConfirmationCodeEntity>(
            e => e.Token == token,
            c => c.ConfirmationCodes
                .Include(code => code.Person)
                .ThenInclude(p => p.Auth));
    }
}