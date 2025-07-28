using AccountService.Common.Abstractions;
using AccountService.Features.Transactions;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.Transfer;

public class TransferCommandHandler(IFakeDataStorage fakeDataStorage, IMediator mediator) : ICommandHandler<TransferCommand, Unit>
{
    public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var account = await fakeDataStorage.GetAccountByIdAsync(request.TransferDto.AccountId);
        var counterpartyAccount = await fakeDataStorage.GetAccountByIdAsync(request.TransferDto.CounterpartyAccountId);

        if(account == null)
            throw AccountNotFoundException.WithSuchId(request.TransferDto.AccountId);
        
        if(counterpartyAccount == null)
            throw AccountNotFoundException.WithSuchId(request.TransferDto.CounterpartyAccountId);
        
        if (request.TransferDto.Type == TransactionType.Debit)
        {
            account.Balance += request.TransferDto.Amount;
            if (counterpartyAccount.Balance - request.TransferDto.Amount < 0)
                throw AccountBadRequestException.InsufficientFundsException(
                    counterpartyAccount.Id,
                    counterpartyAccount.Balance,
                    request.TransferDto.Amount);
            counterpartyAccount.Balance -= request.TransferDto.Amount;
        }
        else
        {
            if (account.Balance - request.TransferDto.Amount < 0)
                throw AccountBadRequestException.InsufficientFundsException(
                    account.Id,
                    account.Balance,
                    request.TransferDto.Amount);
            account.Balance -= request.TransferDto.Amount;
            counterpartyAccount.Balance += request.TransferDto.Amount;
        }
        
        await fakeDataStorage.UpdateAccountAsync(account);
        await fakeDataStorage.UpdateAccountAsync(counterpartyAccount);

        var transaction = new TransactionDto
        {
            AccountId = account.Id,
            CounterpartyAccountId = counterpartyAccount.Id,
            Amount = request.TransferDto.Amount,
            Currency = request.TransferDto.Currency,
            Type = request.TransferDto.Type,
            Description = $"Получил на счет {account.Id} от {counterpartyAccount.Id} сумму {request.TransferDto.Amount}"
        };
        
        await mediator.Send(new CreateTransactionCommand(transaction), cancellationToken);

        transaction.AccountId = counterpartyAccount.Id;
        transaction.CounterpartyAccountId = account.Id;
        transaction.Type = transaction.Type == TransactionType.Debit ? TransactionType.Credit : TransactionType.Debit;
        transaction.Description =
            $"Отправил на счет {account.Id} от {counterpartyAccount.Id} сумму {request.TransferDto.Amount}";
        
        await mediator.Send(new CreateTransactionCommand(transaction), cancellationToken);
            
        return Unit.Value;
    }
}