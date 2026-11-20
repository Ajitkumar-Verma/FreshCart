using FreshCart.Common.Auth;
using FreshCart.Common.Constants;
using FreshCart.IService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

[ApiController]
[Route(ApiRoutes.SuperUsers)]
public sealed class SuperUsersController(IAuthService authService) : ControllerBase
{
    [Authorize(Roles = AppRoles.PlatformAdmin)]
    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Create(CreateSuperUserRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.CreatePlatformAdminAsync(request, cancellationToken));

    [AllowAnonymous]
    [HttpPost(ApiRoutes.BootstrapSuperUser)]
    public async Task<ActionResult<AuthResponse>> Bootstrap(
        CreateSuperUserRequest request,
        [FromHeader(Name = HttpHeaderNames.BootstrapKey)] string? bootstrapKey,
        CancellationToken cancellationToken) =>
        Ok(await authService.BootstrapPlatformAdminAsync(request, bootstrapKey, cancellationToken));
}

