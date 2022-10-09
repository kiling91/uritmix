using Model.Auth;

namespace DataAccess.Auth;

public interface IConfirmationCoderRepository
{
    Task<ConfirmationCodeModel> Create(ConfirmationCodeModel model);
    Task Remove(string token);
    Task Remove(long personId, ConfirmTokenType type);
    Task<ConfirmationCodeModel?> Find(string token);
}