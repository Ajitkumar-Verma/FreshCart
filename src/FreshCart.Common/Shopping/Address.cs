namespace FreshCart.Common.Shopping;

public sealed class Address
{
    public Guid Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string RecipientName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
