using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountMessageHandler(IFakeDataStorage fakeDataStorage) : IMessageHandler<UpdateAccountMessage, Unit>
{
    public async Task<Unit> Handle(UpdateAccountMessage request, CancellationToken cancellationToken)
    {
        var account = await fakeDataStorage.GetAccountByIdAsync(request.AccountId);
        
        if (account == null)
            throw AccountNotFoundException.WithSuchId(request.AccountId);
        
        account.Balance = request.AccountDto.Balance;
        account.InterestRate = request.AccountDto.InterestRate;
        
        await fakeDataStorage.UpdateAccountAsync(account);
        return Unit.Value;
    }
}