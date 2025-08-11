using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountMessageHandler(IAccountRepository accountRepository)
    : IMessageHandler<GetAccountMessage, MbResult<DbAccount>>
{
    public async Task<MbResult<DbAccount>> Handle(GetAccountMessage request, CancellationToken cancellationToken)
    {
        if (!await accountRepository.ExistsAsync(request.Id))
        {
            return MbResult<DbAccount>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.Id} not found"
            ));
        }

        var account = await accountRepository.GetByIdAsync(request.Id);
        if (account is null)
        {
            return MbResult<DbAccount>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.Id} not found"
            ));
        }
        return MbResult<DbAccount>.Success(account);
    }
}