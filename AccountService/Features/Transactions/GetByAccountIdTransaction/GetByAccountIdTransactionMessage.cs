using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public record GetByAccountIdTransactionMessage(Guid AccountId) : IMessage<MbResult<List<DbTransaction>>>;