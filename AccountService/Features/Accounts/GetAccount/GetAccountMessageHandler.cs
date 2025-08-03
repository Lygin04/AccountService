using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountMessageHandler(IFakeDataStorage fakeDataStorage) : IMessageHandler<GetAccountMessage, Account?>
{
    public async Task<Account?> Handle(GetAccountMessage request, CancellationToken cancellationToken)
    {
        if(!await fakeDataStorage.ExistsAccountAsync(request.Id))
            throw AccountNotFoundException.WithSuchId(request.Id);
        
        var account = await fakeDataStorage.GetAccountByIdAsync(request.Id);
        return account;
    }
}