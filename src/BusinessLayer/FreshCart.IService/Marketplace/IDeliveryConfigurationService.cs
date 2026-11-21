using FreshCart.Common.Marketplace;

namespace FreshCart.IService.Marketplace;

public interface IDeliveryConfigurationService
{
    /// <summary>Lists active delivery zones matching a postal code.</summary>
    Task<IReadOnlyList<DeliveryZone>> GetZonesByPostalCodeAsync(string postalCode, CancellationToken cancellationToken = default);

    /// <summary>Lists zones configured for a shop branch after verifying shop management.</summary>
    Task<IReadOnlyList<DeliveryZone>> GetZonesForManagementAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates one delivery zone for a managed branch.</summary>
    Task<DeliveryZone> CreateZoneAsync(Guid shopId, Guid branchId, DeliveryZoneRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one delivery zone after verifying its branch belongs to a managed shop.</summary>
    Task<DeliveryZone> UpdateZoneAsync(Guid shopId, Guid branchId, Guid zoneId, DeliveryZoneRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one delivery zone.</summary>
    Task DeleteZoneAsync(Guid shopId, Guid branchId, Guid zoneId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Lists active slot templates configured for a shop branch.</summary>
    Task<IReadOnlyList<DeliverySlot>> GetSlotsAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates one recurring delivery slot template.</summary>
    Task<DeliverySlot> CreateSlotAsync(Guid shopId, Guid branchId, DeliverySlotRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one recurring delivery slot template.</summary>
    Task<DeliverySlot> UpdateSlotAsync(Guid shopId, Guid branchId, Guid slotId, DeliverySlotRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one delivery slot template.</summary>
    Task DeleteSlotAsync(Guid shopId, Guid branchId, Guid slotId, Guid actorUserId, CancellationToken cancellationToken = default);
}
