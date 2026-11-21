using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Inventory;

public sealed class InventoryAdjustmentRequest
{
    [Range(typeof(decimal), "-999999999", "999999999")]
    public decimal Quantity { get; set; }

    [Required, MaxLength(40)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
