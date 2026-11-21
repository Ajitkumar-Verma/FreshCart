using FreshCart.Common.Shopping;

namespace FreshCart.IRepository.Shopping;

public interface IAddressRepository
{
    /// <summary>Returns active addresses belonging to a user.</summary>
    Task<IReadOnlyList<Address>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Finds one active address owned by a user.</summary>
    Task<Address?> GetByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);

    /// <summary>Inserts an address row for one user.</summary>
    Task CreateAsync(Guid addressId, Guid userId, AddressRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates the editable fields of one active address.</summary>
    Task UpdateAsync(Guid userId, Guid addressId, AddressRequest request, CancellationToken cancellationToken = default);

    /// <summary>Sets one active address as default and clears the other defaults for that user.</summary>
    Task<bool> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one active address owned by a user.</summary>
    Task<bool> SoftDeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
}
