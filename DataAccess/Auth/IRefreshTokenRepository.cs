using Model.Auth;

namespace DataAccess.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel> CreateOrUpdate(RefreshTokenModel model);
    Task<RefreshTokenModel?> Get(long id);
}