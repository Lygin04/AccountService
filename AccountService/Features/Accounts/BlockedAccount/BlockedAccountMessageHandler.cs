using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Infrastructure.Clients.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.BlockedAccount;

public class BlockedAccountMessageHandler(IAccountRepository accountRepository, IClientVerificationService clientVerification) : IMessageHandler<BlockedAccountMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(BlockedAccountMessage request, CancellationToken cancellationToken)
    {
        if (!clientVerification.ClientExists(request.OwnerId))
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Client with OwnerId {request.OwnerId} not found"
            ));
        }
        
        await accountRepository.SetFrozenByOwnerAsync(request.OwnerId, request.IsBlocked);
        
        return MbResult<Unit>.Success(Unit.Value);
    }
}