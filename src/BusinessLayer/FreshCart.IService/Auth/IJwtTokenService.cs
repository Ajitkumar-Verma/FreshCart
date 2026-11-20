using FreshCart.Common.Auth;

namespace FreshCart.IService.Auth;

public interface IJwtTokenService
{
    /// <summary>Issues a signed, expiring access token containing the account ID and role claims.</summary>
    AuthResponse CreateToken(UserAccount user, IReadOnlyList<string> roles);
}
