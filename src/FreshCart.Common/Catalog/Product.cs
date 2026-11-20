namespace FreshCart.Common.Catalog;

public sealed record Product(int ProductId, int ShopId, int CategoryId, string Name, string? Description, decimal Price, string CurrencyCode, bool IsAvailable);
