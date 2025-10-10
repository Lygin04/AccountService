using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Contracts;
using AccountService.Features.Accounts;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Outbox.Interfaces;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public class CreateTransactionMessageHandler(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository,
    IValidator<CreateTransactionMessage> validator,
    IOutboxWriter outboxWriter,
    IDapperContext<IDapperSettings> dapperContext) : IMessageHandler<CreateTransactionMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(CreateTransactionMessage request, CancellationToken cancellationToken)
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
        
        var transaction = new DbTransaction
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
        
        var account = await accountRepository.GetByIdAsync(transaction.AccountId);
        if (account == null)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {transaction.AccountId} not found"
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

        if (transaction.Type == TransactionType.Debit)
        {
            account.Balance += transaction.Amount;
        }
        else
        {
            if (account.Balance - transaction.Amount < 0)
            {
                return MbResult<Unit>.Failure(new MbError(
                    title: "Bad Request",
                    status: StatusCodes.Status400BadRequest,
                    detail: $"Insufficient funds on account {account.Id}. Balance: {account.Balance}, Requested: {transaction.Amount}"
                ));
            }
            account.Balance -= transaction.Amount;
        }

        account.Transactions ??= [];

        using var transactionDb = dapperContext.BeginTransaction();
        try
        {
            await transactionRepository.AddAsync(transaction, transactionDb);

            Event? payload = transaction.Type switch
            {
                TransactionType.Credit => new MoneyCredited(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    account.Id,
                    transaction.Amount,
                    transaction.Currency,
                    transaction.Id),

                TransactionType.Debit => new MoneyDebited(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    account.Id,
                    transaction.Amount,
                    transaction.Currency,
                    transaction.Id,
                    ""), // TODO: Подумать что с этим можно сделать

                _ => null
            };

            if (payload != null)
            {
                await outboxWriter.WriteAsync("account.notifications", payload, transactionDb);
            }
            
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