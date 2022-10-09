using AutoMapper;
using DataAccess.Auth;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess.Exceptions;
using Helpers.DataAccess.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Auth;

namespace DataAccess.Relational.Auth;

public class ConfirmationCoderRepository : RepositoryBase<DbServiceContext>, IConfirmationCoderRepository
{
    public ConfirmationCoderRepository(DbServiceContext context, IMapper mapperObject,
        ILogger<ConfirmationCoderRepository> logger)
        : base(context, mapperObject, logger)
    {
    }

    public Task<ConfirmationCodeModel> Create(ConfirmationCodeModel model)
    {
        return CreateEntity(model, context => context.ConfirmationCodes);
    }

    public async Task Remove(string token)
    {
        await DeleteEntity(
            e => e.Token == token, Dummy<ConfirmationCodeModel>,
            context => context.ConfirmationCodes);
    }

    public async Task Remove(long personId, ConfirmTokenType type)
    {
        await DeleteEntity(
            e => e.PersonId == personId && e.Type == (byte)type,
            Dummy<ConfirmationCodeModel>,
            context => context.ConfirmationCodes,
            false);
    }

    public async Task<ConfirmationCodeModel?> Find(string token)
    {
        return await GetEntity(e => e.Token == token, Dummy<ConfirmationCodeModel>,
            c => c.ConfirmationCodes
                .Include(code => code.Person)
                .ThenInclude(p => p.Auth)
        );
    }
}