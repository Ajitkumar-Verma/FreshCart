using FreshCart.IService.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FreshCart.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAvailable([FromQuery] Guid? shopId, [FromQuery] Guid? categoryId, [FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await products.GetAvailableAsync(shopId, categoryId, search, cancellationToken));

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(productId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }
}

