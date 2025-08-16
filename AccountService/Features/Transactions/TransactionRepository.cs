using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using AccountService.Infrastructure.Scripts.Transaction;

namespace AccountService.Features.Transactions;

public class TransactionRepository(IDapperContext<IDapperSettings> dapperContext) : ITransactionRepository
{
    public async Task AddAsync(DbTransaction dbTransaction, ITransaction? transaction = null)
    {
        await dapperContext.Command(new QueryObject(Transaction.Create, dbTransaction), transaction);
    }

    public async Task<List<DbTransaction>> GetByAccountIdAsync(Guid accountId)
    {
        return await dapperContext.ListOrEmpty<DbTransaction>(new  QueryObject(Transaction.GetByAccountId, accountId));
    }
}