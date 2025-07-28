using AccountService.Common.Abstractions;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandler(IFakeDataStorage fakeDataStorage) : ICommandHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        if (!await fakeDataStorage.ExistsAccountAsync(request.Id))
            throw AccountNotFoundException.WithSuchId(request.Id);
        
        await fakeDataStorage.DeleteAccountAsync(request.Id);
        return Unit.Value;
    }
}