using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountMessageHandler(IFakeDataStorage fakeDataStorage) : IMessageHandler<DeleteAccountMessage, Unit>
{
    public async Task<Unit> Handle(DeleteAccountMessage request, CancellationToken cancellationToken)
    {
        if (!await fakeDataStorage.ExistsAccountAsync(request.Id))
            throw AccountNotFoundException.WithSuchId(request.Id);
        
        await fakeDataStorage.DeleteAccountAsync(request.Id);
        return Unit.Value;
    }
}