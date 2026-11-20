using FreshCart.IService.Catalog;
using FreshCart.Service.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace FreshCart.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddFreshCartServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
