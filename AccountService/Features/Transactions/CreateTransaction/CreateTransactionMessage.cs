using AccountService.Common;
using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public record CreateTransactionMessage(TransactionDto TransactionDto) : IMessage<MbResult<Unit>>;