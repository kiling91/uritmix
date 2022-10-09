namespace Service.Security.UserJwt;

public class ResolveRefreshTokenData
{
    public long TokenId { get; init; }
    public RefreshTokenValidateType Type { get; init; }
}