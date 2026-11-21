using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Shopping;

public sealed class AddressRequest
{
    [Required, MaxLength(60)]
    public string Label { get; set; } = string.Empty;

    [Required, MaxLength(160)]
    public string RecipientName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string AddressLine { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
