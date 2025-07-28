using AccountService.Common.Abstractions;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler(IFakeDataStorage fakeDataStorage) : ICommandHandler<UpdateAccountCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
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