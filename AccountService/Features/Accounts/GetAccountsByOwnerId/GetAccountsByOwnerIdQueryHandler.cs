using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public class GetAccountsByOwnerIdQueryHandler(IFakeDataStorage fakeDataStorage, IClientVerificationService clientVerification) : IQueryHandler<GetAccountsByOwnerIdQuery, List<Account>>
{
    public async Task<List<Account>> Handle(GetAccountsByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        if(!clientVerification.ClientExists(request.OwnerId))
            throw AccountNotFoundException.WithSuchOwnerId(request.OwnerId);
        
        var accounts = await fakeDataStorage.GetAccountByOwnerIdAsync(request.OwnerId);
        return accounts;
    }
}