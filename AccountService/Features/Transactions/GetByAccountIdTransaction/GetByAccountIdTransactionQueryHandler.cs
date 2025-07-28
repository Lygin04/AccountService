using AccountService.Common.Abstractions;
using AccountService.Features.Accounts;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public class GetByAccountIdTransactionQueryHandler(IFakeDataStorage fakeDataStorage) : IQueryHandler<GetByAccountIdTransactionQuery, List<Transaction>?>
{
    public async Task<List<Transaction>?> Handle(GetByAccountIdTransactionQuery request, CancellationToken cancellationToken)
    {
        if(!await fakeDataStorage.ExistsAccountAsync(request.AccountId))
            throw AccountNotFoundException.WithSuchId(request.AccountId);
        
        var transactions = await fakeDataStorage.GetTransactionsByAccountIdAsync(request.AccountId);
        
        return transactions;
    }
}