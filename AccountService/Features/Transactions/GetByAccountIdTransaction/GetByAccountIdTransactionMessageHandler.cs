using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public class GetByAccountIdTransactionMessageHandler(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository)
    : IMessageHandler<GetByAccountIdTransactionMessage, MbResult<List<DbTransaction>>>
{
    public async Task<MbResult<List<DbTransaction>>> Handle(GetByAccountIdTransactionMessage request,
        CancellationToken cancellationToken)
    {
        if (!await accountRepository.ExistsAsync(request.AccountId))
        {
            return MbResult<List<DbTransaction>>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.AccountId} not found"
            ));
        }

        var transactions = await transactionRepository.GetByAccountIdAsync(request.AccountId);

        return MbResult<List<DbTransaction>>.Success(transactions);

    }
}