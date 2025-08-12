using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Dapper.Interfaces;

namespace AccountService.Features.Transactions;

public interface ITransactionRepository
{
    Task AddAsync(DbTransaction dbTransaction, ITransaction? transaction = null);
    // ReSharper disable once UnusedParameter.Global
    Task<List<DbTransaction>> GetByAccountIdAsync(Guid accountId);
}