using FreshCart.Common.Shopping;

namespace FreshCart.IService.Shopping;

public interface IAddressService
{
    /// <summary>Returns delivery addresses belonging to the specified user.</summary>
    Task<IReadOnlyList<Address>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Returns one address belonging to the specified user or raises not found.</summary>
    Task<Address> GetByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);

    /// <summary>Creates an address for the specified user and establishes a default when needed.</summary>
    Task<Address> CreateAsync(Guid userId, AddressRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates one address belonging to the specified user.</summary>
    Task<Address> UpdateAsync(Guid userId, Guid addressId, AddressRequest request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one address and selects a replacement default when necessary.</summary>
    Task DeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);

    /// <summary>Marks one of the user's addresses as the default delivery address.</summary>
    Task<Address> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
}
