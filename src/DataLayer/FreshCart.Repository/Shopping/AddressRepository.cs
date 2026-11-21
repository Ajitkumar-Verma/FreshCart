using Dapper;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using System.Data;

namespace FreshCart.Repository.Shopping;

public sealed class AddressRepository(IUnitOfWork unitOfWork) : IAddressRepository
{
    public async Task<IReadOnlyList<Address>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressGetAll,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Address>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Address?> GetByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressGetById,
                    parameters: new { UserId = userId, AddressId = addressId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Address>(command);
            }, cancellationToken);
    }

    public async Task CreateAsync(Guid addressId, Guid userId, AddressRequest request, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressCreate,
                    parameters: new
                    {
                        AddressId = addressId,
                        UserId = userId,
                        request.Label,
                        request.RecipientName,
                        request.Phone,
                        request.AddressLine,
                        request.City,
                        request.PostalCode,
                        CreatedBy = userId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task UpdateAsync(Guid userId, Guid addressId, AddressRequest request, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressUpdate,
                    parameters: new
                    {
                        UserId = userId,
                        AddressId = addressId,
                        request.Label,
                        request.RecipientName,
                        request.Phone,
                        request.AddressLine,
                        request.City,
                        request.PostalCode,
                        UpdatedBy = userId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressSetDefault,
                    parameters: new { UserId = userId, AddressId = addressId, UpdatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.AddressSoftDelete,
                    parameters: new { UserId = userId, AddressId = addressId, DeletedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}
