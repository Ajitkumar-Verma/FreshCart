using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Auth)]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(ApiRoutes.AuthRegister)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.AuthLogin)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.LoginAsync(request, cancellationToken));
}


