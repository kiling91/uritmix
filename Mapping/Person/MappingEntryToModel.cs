using DataAccess.Relational.Auth.Entities;
using Helpers.Mapping;
using Model.Auth;
using Model.Person;

namespace Mapping.Person;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<PersonModel, PersonEntity>()
            //.IgnoreId()
            .ReverseMapExtended(this);

        CreateMap<AuthModel, AuthEntity>()
            .Map(m => m.Status, m => (byte)m.Status)
            .Map(m => m.Role, m => (byte)m.Role)
            .Ignore(m => m.PersonId)
            .ReverseMapExtended(this)
            .Map(m => m.Status, m => (AuthStatus)m.Status)
            .Map(m => m.Role, m => (AuthRole)m.Role);

        CreateMap<RefreshTokenModel, RefreshTokenEntity>()
            //.IgnoreId()
            .Ignore(m => m.Id)
            .ReverseMapExtended(this);

        CreateMap<ConfirmationCodeModel, ConfirmationCodeEntity>()
            //.IgnoreId()
            .Map(m => m.DateCreate, m => m.DateCreate.ToFileTimeUtc())
            .Map(m => m.Type, m => (byte)m.Type)
            .ReverseMapExtended(this)
            .Map(m => m.DateCreate, m => DateTime.FromFileTimeUtc(m.DateCreate))
            .Map(m => m.Type, m => (ConfirmTokenType)m.Type);
    }
}