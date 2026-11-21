using FreshCart.Common.Marketplace;

namespace FreshCart.IRepository.Marketplace;

public interface IDeliveryConfigurationRepository
{
    /// <summary>Returns active zones for a postal code.</summary>
    Task<IReadOnlyList<DeliveryZone>> GetZonesByPostalCodeAsync(string postalCode, CancellationToken cancellationToken = default);

    /// <summary>Returns zones for a branch.</summary>
    Task<IReadOnlyList<DeliveryZone>> GetZonesByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>Finds one zone for a branch.</summary>
    Task<DeliveryZone?> GetZoneByIdAsync(Guid branchId, Guid zoneId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one delivery zone row.</summary>
    Task<DeliveryZone> CreateZoneAsync(DeliveryZone zone, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one delivery zone row.</summary>
    Task<DeliveryZone?> UpdateZoneAsync(DeliveryZone zone, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one delivery zone row.</summary>
    Task<bool> SoftDeleteZoneAsync(Guid branchId, Guid zoneId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Lists slot templates for one branch.</summary>
    Task<IReadOnlyList<DeliverySlot>> GetSlotsByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>Finds one slot template for a branch.</summary>
    Task<DeliverySlot?> GetSlotByIdAsync(Guid branchId, Guid slotId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one slot template row.</summary>
    Task<DeliverySlot> CreateSlotAsync(DeliverySlot slot, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one slot template row.</summary>
    Task<DeliverySlot?> UpdateSlotAsync(DeliverySlot slot, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one slot template row.</summary>
    Task<bool> SoftDeleteSlotAsync(Guid branchId, Guid slotId, Guid actorUserId, CancellationToken cancellationToken = default);
}
