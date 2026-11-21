using FreshCart.Common.Exceptions;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Marketplace;

namespace FreshCart.Service.Marketplace;

public sealed class DeliveryConfigurationService(
    IDeliveryConfigurationRepository delivery,
    IShopBranchRepository branches,
    IShopRepository shops,
    IIdGenerator idGenerator) : IDeliveryConfigurationService
{
    public Task<IReadOnlyList<DeliveryZone>> GetZonesByPostalCodeAsync(string postalCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ValidationAppException();
        return delivery.GetZonesByPostalCodeAsync(postalCode.Trim(), cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryZone>> GetZonesForManagementAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        return await delivery.GetZonesByBranchAsync(branchId, cancellationToken);
    }

    public async Task<DeliveryZone> CreateZoneAsync(Guid shopId, Guid branchId, DeliveryZoneRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        ValidateZone(request);
        return await delivery.CreateZoneAsync(MapZone(request, idGenerator.NewId(), branchId), actorUserId, cancellationToken);
    }

    public async Task<DeliveryZone> UpdateZoneAsync(Guid shopId, Guid branchId, Guid zoneId, DeliveryZoneRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        ValidateZone(request);
        return await delivery.UpdateZoneAsync(MapZone(request, zoneId, branchId), actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteZoneAsync(Guid shopId, Guid branchId, Guid zoneId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        if (!await delivery.SoftDeleteZoneAsync(branchId, zoneId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    public async Task<IReadOnlyList<DeliverySlot>> GetSlotsAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        return await delivery.GetSlotsByBranchAsync(branchId, cancellationToken);
    }

    public async Task<DeliverySlot> CreateSlotAsync(Guid shopId, Guid branchId, DeliverySlotRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        ValidateSlot(request);
        return await delivery.CreateSlotAsync(MapSlot(request, idGenerator.NewId(), branchId), actorUserId, cancellationToken);
    }

    public async Task<DeliverySlot> UpdateSlotAsync(Guid shopId, Guid branchId, Guid slotId, DeliverySlotRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        ValidateSlot(request);
        return await delivery.UpdateSlotAsync(MapSlot(request, slotId, branchId), actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteSlotAsync(Guid shopId, Guid branchId, Guid slotId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureBranchAccessAsync(shopId, branchId, actorUserId, cancellationToken);
        if (!await delivery.SoftDeleteSlotAsync(branchId, slotId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureBranchAccessAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        if (await branches.GetByIdAsync(shopId, branchId, cancellationToken) is null)
            throw new NotFoundAppException();
    }

    private static void ValidateZone(DeliveryZoneRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.PostalCode)
            || request.DeliveryFee < 0 || request.MinimumOrder < 0)
            throw new ValidationAppException();
    }

    private static void ValidateSlot(DeliverySlotRequest request)
    {
        if (request.StartsAt < TimeSpan.Zero || request.EndsAt <= request.StartsAt || request.EndsAt >= TimeSpan.FromDays(1) || request.Capacity <= 0)
            throw new ValidationAppException();
    }

    private static DeliveryZone MapZone(DeliveryZoneRequest request, Guid id, Guid branchId) => new()
    {
        Id = id,
        ShopBranchId = branchId,
        Name = request.Name.Trim(),
        PostalCode = request.PostalCode.Trim(),
        DeliveryFee = request.DeliveryFee,
        MinimumOrder = request.MinimumOrder,
        IsActive = request.IsActive
    };

    private static DeliverySlot MapSlot(DeliverySlotRequest request, Guid id, Guid branchId) => new()
    {
        Id = id,
        ShopBranchId = branchId,
        StartsAt = request.StartsAt,
        EndsAt = request.EndsAt,
        Capacity = request.Capacity,
        IsActive = request.IsActive
    };
}
