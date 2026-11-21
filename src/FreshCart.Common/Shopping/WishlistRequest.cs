using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Shopping;

public sealed class WishlistRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
