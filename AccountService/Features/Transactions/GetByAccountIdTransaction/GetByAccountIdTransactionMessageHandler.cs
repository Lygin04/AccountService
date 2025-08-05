using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public class GetByAccountIdTransactionMessageHandler(IFakeDataStorage fakeDataStorage)
    : IMessageHandler<GetByAccountIdTransactionMessage, MbResult<List<Transaction>>>
{
    public async Task<MbResult<List<Transaction>>> Handle(GetByAccountIdTransactionMessage request,
        CancellationToken cancellationToken)
    {
        if (!await fakeDataStorage.ExistsAccountAsync(request.AccountId))
        {
            return MbResult<List<Transaction>>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.AccountId} not found"
            ));
        }

        var transactions = await fakeDataStorage.GetTransactionsByAccountIdAsync(request.AccountId);

        return MbResult<List<Transaction>>.Success(transactions);

    }
}