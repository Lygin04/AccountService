using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public class GetByAccountIdTransactionMessageHandler(IFakeDataStorage fakeDataStorage) : IMessageHandler<GetByAccountIdTransactionMessage, List<Transaction>?>
{
    public async Task<List<Transaction>?> Handle(GetByAccountIdTransactionMessage request, CancellationToken cancellationToken)
    {
        if(!await fakeDataStorage.ExistsAccountAsync(request.AccountId))
            throw AccountNotFoundException.WithSuchId(request.AccountId);
        
        var transactions = await fakeDataStorage.GetTransactionsByAccountIdAsync(request.AccountId);
        
        return transactions;
    }
}