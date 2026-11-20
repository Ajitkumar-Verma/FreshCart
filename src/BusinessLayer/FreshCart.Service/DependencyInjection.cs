using FreshCart.Common.Auth;
using FreshCart.IService.Auth;
using FreshCart.IService.Catalog;
using FreshCart.Service.Auth;
using FreshCart.Service.Catalog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FreshCart.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddFreshCartServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        return services;
    }
}
