using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public class GetAccountsByOwnerIdMessageHandler(
    IFakeDataStorage fakeDataStorage,
    IClientVerificationService clientVerification) : IMessageHandler<GetAccountsByOwnerIdMessage, MbResult<List<Account>>>
{
    public async Task<MbResult<List<Account>>> Handle(GetAccountsByOwnerIdMessage request, CancellationToken cancellationToken)
    {
        if (!clientVerification.ClientExists(request.OwnerId))
        {
            return MbResult<List<Account>>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Client with OwnerId {request.OwnerId} not found"
            ));
        }

        var accounts = await fakeDataStorage.GetAccountByOwnerIdAsync(request.OwnerId);
        return MbResult<List<Account>>.Success(accounts);
    }
}