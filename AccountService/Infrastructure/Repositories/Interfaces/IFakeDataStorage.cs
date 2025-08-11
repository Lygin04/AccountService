using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.Models;

namespace AccountService.Infrastructure.Repositories.Interfaces;

public interface IFakeDataStorage
{
    Task<DbAccount?> GetAccountByIdAsync(Guid id);
    Task<List<DbAccount>> GetAccountByOwnerIdAsync(Guid ownerId);
    Task<List<DbTransaction>> GetTransactionsByAccountIdAsync(Guid accountId);

    Task AddAccountAsync(DbAccount dbAccount);
    Task UpdateAccountAsync(DbAccount dbAccount);
    Task DeleteAccountAsync(Guid accountId);
    Task AddTransactionAsync(DbTransaction dbTransaction);
    Task<bool> ExistsAccountAsync(Guid accountId);
}