namespace FreshCart.Common.Persistence;

/// <summary>Creates stable identifiers before persistence so related rows can share the same key.</summary>
public interface IIdGenerator
{
    Guid NewId();
}
