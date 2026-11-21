using Dapper;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using System.Data;

namespace FreshCart.Repository.Shopping;

public sealed class CouponRepository(IUnitOfWork unitOfWork) : ICouponRepository
{
    public async Task<IReadOnlyList<Coupon>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.GetAll,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Coupon>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.GetByCode,
                    parameters: new { Code = code },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Coupon>(command);
            }, cancellationToken);
    }

    public async Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.GetById,
                    parameters: new { CouponId = couponId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Coupon>(command);
            }, cancellationToken);
    }

    public async Task<Coupon> CreateAsync(Coupon coupon, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.Create,
                    parameters: new { coupon.Id, coupon.ShopId, coupon.Code, coupon.DiscountType, coupon.DiscountValue, coupon.MinimumOrder, coupon.StartsAt, coupon.EndsAt, coupon.UsageLimit, coupon.IsActive, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<Coupon>(command);
            }, cancellationToken);
    }

    public async Task<Coupon?> UpdateAsync(Coupon coupon, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.Update,
                    parameters: new { coupon.Id, coupon.ShopId, coupon.Code, coupon.DiscountType, coupon.DiscountValue, coupon.MinimumOrder, coupon.StartsAt, coupon.EndsAt, coupon.UsageLimit, coupon.IsActive, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Coupon>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid couponId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Coupons.SoftDelete,
                    parameters: new { CouponId = couponId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}
