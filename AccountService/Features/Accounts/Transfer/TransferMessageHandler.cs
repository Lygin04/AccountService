using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Contracts;
using AccountService.Features.Accounts.UpdateAccount;
using AccountService.Features.Transactions;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Outbox.Interfaces;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.Transfer;

public class TransferMessageHandler(
    IAccountRepository accountRepository,
    IMediator mediator,
    IDapperContext<IDapperSettings> dapperContext,
    IValidator<TransferMessage> validator,
    IOutboxWriter outboxWriter) : IMessageHandler<TransferMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(TransferMessage request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            
            return MbResult<Unit>.Failure(new MbError(
                title: "Validation Error",
                status: StatusCodes.Status422UnprocessableEntity,
                detail: "Validation failed",
                errors: errors
            ));
        }
        
        var account = await accountRepository.GetByIdAsync(request.TransferDto.AccountId);
        var counterpartyAccount = await accountRepository.GetByIdAsync(request.TransferDto.CounterpartyAccountId);

        if (account == null)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.TransferDto.AccountId} not found"
            ));
        }
        
        if (account.IsBlocked)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Account Blocked",
                status: StatusCodes.Status409Conflict,
                detail: $"Account with ID {account.Id} blocked"
            ));
        }

        if (counterpartyAccount == null)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Counterparty account with ID {request.TransferDto.CounterpartyAccountId} not found"
            ));
        }
        
        if (account.IsBlocked)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Account Blocked",
                status: StatusCodes.Status409Conflict,
                detail: $"Account with ID {counterpartyAccount.Id} blocked"
            ));
        }
        
        if (request.TransferDto.Type == TransactionType.Debit)
        {
            account.Balance += request.TransferDto.Amount;
            if (counterpartyAccount.Balance - request.TransferDto.Amount < 0)
            {   return MbResult<Unit>.Failure(new MbError(
                    title: "Bad Request",
                    status: StatusCodes.Status400BadRequest,
                    detail: $"Insufficient funds on account {counterpartyAccount.Id}. Balance: {counterpartyAccount.Balance}, Requested: {request.TransferDto.Amount}"
                ));
            }
            counterpartyAccount.Balance -= request.TransferDto.Amount;
        }
        else
        {
            if (account.Balance - request.TransferDto.Amount < 0)
            {   return MbResult<Unit>.Failure(new MbError(
                    title: "Bad Request",
                    status: StatusCodes.Status400BadRequest,
                    detail: $"Insufficient funds on account {account.Id}. Balance: {account.Balance}, Requested: {request.TransferDto.Amount}"
                ));
            }
            account.Balance -= request.TransferDto.Amount;
            counterpartyAccount.Balance += request.TransferDto.Amount;
        }

        var accountUpdate = new UpdateAccountResponseDto
        {
            Balance = account.Balance,
            InterestRate = account.InterestRate
        };

        var counterpartyAccountUpdate = new UpdateAccountResponseDto
        {
            Balance = counterpartyAccount.Balance,
            InterestRate = counterpartyAccount.InterestRate
        };
        
        using var transactionDb = dapperContext.BeginTransaction();

        try
        {
            var updated = await accountRepository.UpdateAsync(account.Id, accountUpdate, transactionDb);

            if (!updated)
            {
                transactionDb.Rollback();
                return MbResult<Unit>.Failure(new MbError(
                    title: "Conflict",
                    status: StatusCodes.Status409Conflict,
                    detail: $"Conflict detected while updating account {account.Id}. The record was modified by another process."
                ));  
            }
            
            updated = await accountRepository.UpdateAsync(counterpartyAccount.Id, counterpartyAccountUpdate, transactionDb);
            
            if (!updated)
            {
                transactionDb.Rollback();
                return MbResult<Unit>.Failure(new MbError(
                    title: "Conflict",
                    status: StatusCodes.Status409Conflict,
                    detail: $"Conflict detected while updating account {account.Id}. The record was modified by another process."
                ));   
            }
            
            var transaction = new TransactionDto
            {
                AccountId = account.Id,
                CounterpartyAccountId = counterpartyAccount.Id,
                Amount = request.TransferDto.Amount,
                Currency = request.TransferDto.Currency,
                Type = request.TransferDto.Type,
                Description =
                    $"Получил на счет {account.Id} от {counterpartyAccount.Id} сумму {request.TransferDto.Amount}"
            };

            await mediator.Send(new CreateTransactionMessage(transaction), cancellationToken);

            transaction.AccountId = counterpartyAccount.Id;
            transaction.CounterpartyAccountId = account.Id;
            transaction.Type = transaction.Type == TransactionType.Debit
                ? TransactionType.Credit
                : TransactionType.Debit;
            transaction.Description =
                $"Отправил на счет {account.Id} от {counterpartyAccount.Id} сумму {request.TransferDto.Amount}";

            await mediator.Send(new CreateTransactionMessage(transaction), cancellationToken);

            var payload = new TransferCompleted(
                Guid.NewGuid(),
                DateTime.UtcNow,
                account.Id,
                counterpartyAccount.Id,
                request.TransferDto.Amount,
                request.TransferDto.Currency,
                Guid.NewGuid());        // TODO: Подумать что с этим делать

            await outboxWriter.WriteAsync("account.audit", payload, transactionDb);
            
            transactionDb.Commit();
        }
        catch
        {
            transactionDb.Rollback();
            throw;
        }
            
        return MbResult<Unit>.Success(Unit.Value);
    }
}