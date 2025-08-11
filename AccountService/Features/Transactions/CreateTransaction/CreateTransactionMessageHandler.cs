using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts;
using AccountService.Features.Transactions.Models;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public class CreateTransactionMessageHandler(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository,
    IValidator<CreateTransactionMessage> validator) : IMessageHandler<CreateTransactionMessage, MbResult<Unit>>
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

        await transactionRepository.AddAsync(transaction);
        
        return MbResult<Unit>.Success(Unit.Value);
    }
}