using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountMessageHandler(IFakeDataStorage fakeDataStorage)
    : IMessageHandler<GetAccountMessage, MbResult<Account>>
{
    public async Task<MbResult<Account>> Handle(GetAccountMessage request, CancellationToken cancellationToken)
    {
        if (!await fakeDataStorage.ExistsAccountAsync(request.Id))
        {
            return MbResult<Account>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.Id} not found"
            ));
        }

        var account = await fakeDataStorage.GetAccountByIdAsync(request.Id);
        if (account is null)
        {
            return MbResult<Account>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.Id} not found"
            ));
        }
        return MbResult<Account>.Success(account);
    }
}