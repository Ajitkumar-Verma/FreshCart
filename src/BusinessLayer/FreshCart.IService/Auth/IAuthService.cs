using FreshCart.Common.Auth;

namespace FreshCart.IService.Auth;

public interface IAuthService
{
    /// <summary>Registers a customer account and returns its signed access token.</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Verifies credentials and returns a signed access token for an active account.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a platform administrator when called by an existing platform administrator.</summary>
    Task<AuthResponse> CreatePlatformAdminAsync(CreateSuperUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates the first platform administrator once, when the deployment bootstrap secret matches.</summary>
    Task<AuthResponse> BootstrapPlatformAdminAsync(CreateSuperUserRequest request, string? suppliedKey, CancellationToken cancellationToken = default);
}
