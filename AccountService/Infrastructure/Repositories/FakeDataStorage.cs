using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Infrastructure.Repositories;

public class FakeDataStorage : IFakeDataStorage
{
    private readonly List<Transaction> _transactions;
    private readonly List<Account> _accounts;

    public FakeDataStorage()
    {
        var account1 = new Account
        {
            Id = Guid.Parse("0bb3945d-9108-4e95-9718-a09c9e621894"),
            OwnerId = Guid.NewGuid(),
            Type = AccountType.Checking,
            Currency = IsoCurrency.Rub,
            Balance = 15000.50m,
            OpenDate = DateTime.UtcNow.AddMonths(-6),
            Transactions = []
        };

        var account2 = new Account
        {
            Id = Guid.Parse("3fa076cd-e65d-4aa1-869f-6c0419900658"),
            OwnerId = Guid.NewGuid(),
            Type = AccountType.Credit,
            Currency = IsoCurrency.Usd,
            Balance = 3200.75m,
            InterestRate = 15.5m,
            OpenDate = DateTime.UtcNow.AddYears(-1),
            Transactions = []
        };

        var account3 = new Account
        {
            Id = Guid.Parse("da739102-a7b4-409c-b163-6bd03a51a18a"),
            OwnerId = Guid.NewGuid(),
            Type = AccountType.Deposit,
            Currency = IsoCurrency.Eur,
            Balance = 5000m,
            InterestRate = 5.5m,
            OpenDate = DateTime.UtcNow.AddMonths(-12),
            Transactions = []
        };

        _accounts = [account1, account2, account3];

        _transactions =
        [
            new Transaction
            {
                Id = Guid.Parse("f1d611f7-3484-4193-afad-91ae9ef95829"),
                AccountId = account1.Id,
                CounterpartyAccountId = account2.Id,
                Amount = 1000m,
                Currency = IsoCurrency.Rub,
                Type = TransactionType.Debit,
                Description = "Перевод на кредитный счёт",
                Timestamp = DateTime.UtcNow.AddDays(-2)
            },
            new Transaction
            {
                Id = Guid.Parse("468a5c22-b77b-471f-ae84-eecb21cae33b"),
                AccountId = account2.Id,
                CounterpartyAccountId = account1.Id,
                Amount = 500m,
                Currency = IsoCurrency.Usd,
                Type = TransactionType.Credit,
                Description = "Погашение кредита",
                Timestamp = DateTime.UtcNow.AddDays(-1)
            },
            new Transaction
            {
                Id = Guid.Parse("258384df-f841-4810-960e-340eb605b40a"),
                AccountId = account3.Id,
                CounterpartyAccountId = account1.Id,
                Amount = 200m,
                Currency = IsoCurrency.Eur,
                Type = TransactionType.Debit,
                Description = "Пополнение депозита",
                Timestamp = DateTime.UtcNow.AddDays(-5)
            }
        ];

        // Привязываем транзакции к аккаунтам
        foreach (var acc in _accounts)
        {
            acc.Transactions = _transactions.Where(t => t.AccountId == acc.Id).ToList();
        }
    }
    
    #region Queries
    //public async Task<List<Account>> GetAccountsAsync() => await Task.FromResult(_accounts);

    public async Task<Account?> GetAccountByIdAsync(Guid id) =>
        await Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
    
    public async Task<List<Account>> GetAccountByOwnerIdAsync(Guid ownerId) =>
        await Task.FromResult(_accounts.Where(x => x.OwnerId == ownerId).ToList());

    //public async Task<List<Transaction>> GetTransactionsAsync() => await Task.FromResult(_transactions);

    public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId) =>
        await Task.FromResult(_transactions.Where(t => t.AccountId == accountId).ToList());
    #endregion

    #region Commands
    public async Task AddAccountAsync(Account account)
    {
        if (_accounts.All(a => a.Id != account.Id))
            _accounts.Add(account);

        await Task.CompletedTask;
    }

    public async Task UpdateAccountAsync(Account account)
    {
        var existing = _accounts.FirstOrDefault(a => a.Id == account.Id);
        if (existing != null)
        {
            existing.OwnerId = account.OwnerId;
            existing.Type = account.Type;
            existing.Currency = account.Currency;
            existing.Balance = account.Balance;
            existing.InterestRate = account.InterestRate;
            existing.OpenDate = account.OpenDate;
            existing.CloseDate = account.CloseDate;
            existing.Transactions = account.Transactions;
        }

        await Task.CompletedTask;
    }

    public async Task DeleteAccountAsync(Guid accountId)
    {
        _accounts.RemoveAll(a => a.Id == accountId);
        _transactions.RemoveAll(t => t.AccountId == accountId);
        
        await Task.CompletedTask;
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        if (_transactions.Any(t => t.Id == transaction.Id)) return;
        _transactions.Add(transaction);
        var account = _accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
        account?.Transactions?.Add(transaction);
        
        await Task.CompletedTask;
    }

    public Task<bool> ExistsAccountAsync(Guid accountId)
    {
        return Task.FromResult(_accounts.Any(a => a.Id == accountId));
    }
    #endregion
}