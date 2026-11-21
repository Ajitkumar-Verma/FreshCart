using Dapper;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using System.Data;

namespace FreshCart.Repository.Marketplace;

public sealed class DeliveryConfigurationRepository(IUnitOfWork unitOfWork) : IDeliveryConfigurationRepository
{
    public Task<IReadOnlyList<DeliveryZone>> GetZonesByPostalCodeAsync(string postalCode, CancellationToken cancellationToken = default) => QueryManyAsync<DeliveryZone>(StoredProcedure.Marketplace.DeliveryZoneGetByPostalCode, new { PostalCode = postalCode }, cancellationToken);

    public Task<IReadOnlyList<DeliveryZone>> GetZonesByBranchAsync(Guid branchId, CancellationToken cancellationToken = default) => QueryManyAsync<DeliveryZone>(StoredProcedure.Marketplace.DeliveryZoneGetByBranch, new { ShopBranchId = branchId }, cancellationToken);

    public Task<DeliveryZone?> GetZoneByIdAsync(Guid branchId, Guid zoneId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliveryZone>(StoredProcedure.Marketplace.DeliveryZoneGetById, new { ShopBranchId = branchId, ZoneId = zoneId }, cancellationToken);

    public Task<DeliveryZone> CreateZoneAsync(DeliveryZone zone, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliveryZone>(StoredProcedure.Marketplace.DeliveryZoneCreate, new { zone.Id, zone.ShopBranchId, zone.Name, zone.PostalCode, zone.DeliveryFee, zone.MinimumOrder, zone.IsActive, CreatedBy = actorUserId }, cancellationToken)!;

    public Task<DeliveryZone?> UpdateZoneAsync(DeliveryZone zone, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliveryZone>(StoredProcedure.Marketplace.DeliveryZoneUpdate, new { zone.Id, zone.ShopBranchId, zone.Name, zone.PostalCode, zone.DeliveryFee, zone.MinimumOrder, zone.IsActive, UpdatedBy = actorUserId }, cancellationToken);

    public Task<bool> SoftDeleteZoneAsync(Guid branchId, Guid zoneId, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<bool>(StoredProcedure.Marketplace.DeliveryZoneSoftDelete, new { ShopBranchId = branchId, ZoneId = zoneId, DeletedBy = actorUserId }, cancellationToken)!;

    public Task<IReadOnlyList<DeliverySlot>> GetSlotsByBranchAsync(Guid branchId, CancellationToken cancellationToken = default) => QueryManyAsync<DeliverySlot>(StoredProcedure.Marketplace.DeliverySlotGetByBranch, new { ShopBranchId = branchId }, cancellationToken);

    public Task<DeliverySlot?> GetSlotByIdAsync(Guid branchId, Guid slotId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliverySlot>(StoredProcedure.Marketplace.DeliverySlotGetById, new { ShopBranchId = branchId, SlotId = slotId }, cancellationToken);

    public Task<DeliverySlot> CreateSlotAsync(DeliverySlot slot, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliverySlot>(StoredProcedure.Marketplace.DeliverySlotCreate, new { slot.Id, slot.ShopBranchId, slot.StartsAt, slot.EndsAt, slot.Capacity, slot.IsActive, CreatedBy = actorUserId }, cancellationToken)!;

    public Task<DeliverySlot?> UpdateSlotAsync(DeliverySlot slot, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<DeliverySlot>(StoredProcedure.Marketplace.DeliverySlotUpdate, new { slot.Id, slot.ShopBranchId, slot.StartsAt, slot.EndsAt, slot.Capacity, slot.IsActive, UpdatedBy = actorUserId }, cancellationToken);

    public Task<bool> SoftDeleteSlotAsync(Guid branchId, Guid slotId, Guid actorUserId, CancellationToken cancellationToken = default) => QuerySingleAsync<bool>(StoredProcedure.Marketplace.DeliverySlotSoftDelete, new { ShopBranchId = branchId, SlotId = slotId, DeletedBy = actorUserId }, cancellationToken)!;

    private async Task<IReadOnlyList<T>> QueryManyAsync<T>(string procedure, object parameters, CancellationToken cancellationToken)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: procedure,
                    parameters: parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<T>(command)).AsList();
            }, cancellationToken);
    }

    private async Task<T?> QuerySingleAsync<T>(string procedure, object parameters, CancellationToken cancellationToken)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: procedure,
                    parameters: parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<T>(command);
            }, cancellationToken);
    }
}

