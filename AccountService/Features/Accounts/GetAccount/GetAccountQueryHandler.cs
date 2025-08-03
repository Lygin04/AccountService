using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountQueryHandler(IFakeDataStorage fakeDataStorage) : IQueryHandler<GetAccountQuery, Account?>
{
    public async Task<Account?> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        if(!await fakeDataStorage.ExistsAccountAsync(request.Id))
            throw AccountNotFoundException.WithSuchId(request.Id);
        
        var account = await fakeDataStorage.GetAccountByIdAsync(request.Id);
        return account;
    }
}