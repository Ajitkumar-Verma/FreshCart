using FreshCart.IService.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace FreshCart.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAvailable([FromQuery] int? shopId, [FromQuery] int? categoryId, [FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await products.GetAvailableAsync(shopId, categoryId, search, cancellationToken));

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetById(int productId, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(productId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }
}
