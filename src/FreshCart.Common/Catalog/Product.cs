namespace FreshCart.Common.Catalog;

public sealed record Product(Guid ProductId, Guid ShopId, Guid CategoryId, string Name, string? Description, decimal Price, string CurrencyCode, bool IsAvailable);
