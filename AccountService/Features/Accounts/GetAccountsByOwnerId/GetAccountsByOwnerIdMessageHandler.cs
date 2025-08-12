using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public class GetAccountsByOwnerIdMessageHandler(
    IAccountRepository accountRepository,
    IClientVerificationService clientVerification) : IMessageHandler<GetAccountsByOwnerIdMessage, MbResult<List<DbAccount>>>
{
    public async Task<MbResult<List<DbAccount>>> Handle(GetAccountsByOwnerIdMessage request, CancellationToken cancellationToken)
    {
        if (!clientVerification.ClientExists(request.OwnerId))
        {
            return MbResult<List<DbAccount>>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Client with OwnerId {request.OwnerId} not found"
            ));
        }

        var accounts = await accountRepository.GetByOwnerIdAsync(request.OwnerId);
        return MbResult<List<DbAccount>>.Success(accounts);
    }
}