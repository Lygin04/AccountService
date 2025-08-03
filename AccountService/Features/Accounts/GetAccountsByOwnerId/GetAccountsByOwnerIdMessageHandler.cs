using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public class GetAccountsByOwnerIdMessageHandler(
    IFakeDataStorage fakeDataStorage,
    IClientVerificationService clientVerification) : IMessageHandler<GetAccountsByOwnerIdMessage, List<Account>>
{
    public async Task<List<Account>> Handle(GetAccountsByOwnerIdMessage request, CancellationToken cancellationToken)
    {
        if (!clientVerification.ClientExists(request.OwnerId))
            throw AccountNotFoundException.WithSuchOwnerId(request.OwnerId);

        var accounts = await fakeDataStorage.GetAccountByOwnerIdAsync(request.OwnerId);
        return accounts;
    }
}