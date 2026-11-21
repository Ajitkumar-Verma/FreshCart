using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Shopping;

public sealed class CartItemRequest
{
    [Range(typeof(decimal), "0.001", "9999999")]
    public decimal Quantity { get; set; }
}
