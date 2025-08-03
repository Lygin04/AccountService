using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public class CreateTransactionMessageHandler(IFakeDataStorage fakeDataStorage) : IMessageHandler<CreateTransactionMessage, Unit>
{
    public async Task<Unit> Handle(CreateTransactionMessage request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = request.TransactionDto.AccountId,
            CounterpartyAccountId = request.TransactionDto.CounterpartyAccountId,
            Amount = request.TransactionDto.Amount,
            Currency = request.TransactionDto.Currency,
            Type = request.TransactionDto.Type,
            Description = request.TransactionDto.Description,
            Timestamp = DateTime.UtcNow
        };
        
        var account = await fakeDataStorage.GetAccountByIdAsync(transaction.AccountId);
        if (account == null)
            throw AccountNotFoundException.WithSuchId(transaction.AccountId);

        if (transaction.Type == TransactionType.Debit)
        {
            account.Balance += transaction.Amount;
        }
        else
        {
            if (account.Balance - transaction.Amount < 0)
                throw AccountBadRequestException.InsufficientFundsException(account.Id, account.Balance,
                    transaction.Amount);
            account.Balance -= transaction.Amount;
        }

        account.Transactions ??= [];

        await fakeDataStorage.AddTransactionAsync(transaction);
        
        return Unit.Value;
    }
}