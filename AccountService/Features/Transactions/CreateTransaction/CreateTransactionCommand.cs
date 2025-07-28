using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public record CreateTransactionCommand(TransactionDto TransactionDto) : ICommand<Unit>;