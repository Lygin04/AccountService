using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountMessageHandler(IAccountRepository accountRepository)
    : IMessageHandler<DeleteAccountMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(DeleteAccountMessage request, CancellationToken cancellationToken)
    {
        if (!await accountRepository.ExistsAsync(request.Id))
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.Id} not found"
            ));
        }

        await accountRepository.DeleteAsync(request.Id);
        return MbResult<Unit>.Success(Unit.Value);
    }
}