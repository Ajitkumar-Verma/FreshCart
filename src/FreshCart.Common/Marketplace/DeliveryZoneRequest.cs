using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Marketplace;

public sealed class DeliveryZoneRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999")]
    public decimal DeliveryFee { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal MinimumOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
