using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using FreshCart.IService.Shopping;

namespace FreshCart.Service.Shopping;

public sealed class AddressService(IAddressRepository addresses, IIdGenerator idGenerator, IUnitOfWork unitOfWork) : IAddressService
{
    public async Task<IReadOnlyList<Address>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await addresses.GetAllAsync(userId, cancellationToken);
    }

    public async Task<Address> GetByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        return await addresses.GetByIdAsync(userId, addressId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<Address> CreateAsync(Guid userId, AddressRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await addresses.GetAllAsync(userId, cancellationToken);
        var addressId = idGenerator.NewId();
        request.Label = request.Label.Trim();
        request.RecipientName = request.RecipientName.Trim();
        request.Phone = request.Phone.Trim();
        request.AddressLine = request.AddressLine.Trim();
        request.City = request.City.Trim();
        request.PostalCode = request.PostalCode.Trim();

        await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            await addresses.CreateAsync(addressId, userId, request, cancellationToken);
            if (request.IsDefault || existing.Count == 0)
                await addresses.SetDefaultAsync(userId, addressId, cancellationToken);
            var created = await GetByIdAsync(userId, addressId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return created;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<Address> UpdateAsync(Guid userId, Guid addressId, AddressRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(userId, addressId, cancellationToken);
        request.Label = request.Label.Trim();
        request.RecipientName = request.RecipientName.Trim();
        request.Phone = request.Phone.Trim();
        request.AddressLine = request.AddressLine.Trim();
        request.City = request.City.Trim();
        request.PostalCode = request.PostalCode.Trim();

        await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            await addresses.UpdateAsync(userId, addressId, request, cancellationToken);
            if (request.IsDefault && !existing.IsDefault)
                await addresses.SetDefaultAsync(userId, addressId, cancellationToken);
            var updated = await GetByIdAsync(userId, addressId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return updated;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(userId, addressId, cancellationToken);
        await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            if (!await addresses.SoftDeleteAsync(userId, addressId, cancellationToken))
                throw new NotFoundAppException();

            if (existing.IsDefault)
            {
                var nextDefault = (await addresses.GetAllAsync(userId, cancellationToken)).FirstOrDefault();
                if (nextDefault is not null)
                    await addresses.SetDefaultAsync(userId, nextDefault.Id, cancellationToken);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<Address> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        if (!await addresses.SetDefaultAsync(userId, addressId, cancellationToken))
            throw new NotFoundAppException();
        return await GetByIdAsync(userId, addressId, cancellationToken);
    }
}
