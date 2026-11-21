using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Marketplace;

public sealed class ShopMembershipRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(40)]
    public string RoleName { get; set; } = string.Empty;
}
