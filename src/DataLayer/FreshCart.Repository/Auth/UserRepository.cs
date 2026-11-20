using Dapper;
using FreshCart.Common.Auth;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Auth;
using System.Data;

namespace FreshCart.Repository.Auth;

public sealed class UserRepository(IUnitOfWork unitOfWork) : IUserRepository
{
    public async Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.UserGetByEmail,
                    parameters: new
                    {
                        NormalizedEmail = normalizedEmail
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<UserAccount>(command);
            },
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.UserGetRoles,
                    parameters: new
                    {
                        UserId = userId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<string>(command)).AsList();
            },
            cancellationToken: cancellationToken);
    }

    public async Task<Guid?> GetRoleIdByNormalizedNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.RoleGetByNormalizedName,
                    parameters: new
                    {
                        NormalizedName = normalizedRoleName
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Guid?>(command);
            },
            cancellationToken: cancellationToken);
    }

    public async Task<bool> HasActiveUserInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.UserHasActiveRole,
                    parameters: new
                    {
                        RoleId = roleId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            },
            cancellationToken: cancellationToken);
    }

    public async Task CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.UserCreate,
                    parameters: new
                    {
                        user.Id,
                        user.Email,
                        user.NormalizedEmail,
                        user.UserName,
                        user.NormalizedUserName,
                        user.DisplayName,
                        user.PhoneNumber,
                        user.PasswordHash
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            },
            cancellationToken: cancellationToken);
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Auth.UserRoleCreate,
                    parameters: new
                    {
                        UserId = userId,
                        RoleId = roleId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            },
            cancellationToken: cancellationToken);
    }
}
