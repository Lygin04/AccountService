using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.Models;

namespace AccountService.Infrastructure.Repositories.Interfaces;

public interface IFakeDataStorage
{
    Task<Account?> GetAccountByIdAsync(Guid id);
    Task<List<Account>> GetAccountByOwnerIdAsync(Guid ownerId);
    Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId);

    Task AddAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task DeleteAccountAsync(Guid accountId);
    Task AddTransactionAsync(Transaction transaction);
    Task<bool> ExistsAccountAsync(Guid accountId);
}