using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.UpdateAccount;
using AccountService.Infrastructure.Dapper.Interfaces;

namespace AccountService.Features.Accounts;

public interface IAccountRepository
{
    Task AddAsync(DbAccount dbAccount, ITransaction? transaction = null);
    Task<bool> UpdateAsync(Guid accountId, UpdateAccountResponseDto accountDto, ITransaction? transaction = null);
    Task DeleteAsync(Guid accountId, ITransaction? transaction = null);
    Task<DbAccount?> GetByIdAsync(Guid id);
    Task<List<DbAccount>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<Guid>> GetAllAccountIdsAsync();
    Task<bool> ExistsAsync(Guid accountId);
    Task SetFrozenByOwnerAsync(Guid ownerId, bool frozen, ITransaction? transaction = null);
}