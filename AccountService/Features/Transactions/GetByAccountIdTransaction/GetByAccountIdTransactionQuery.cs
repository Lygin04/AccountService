using AccountService.Common.Abstractions;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Transactions.GetByAccountIdTransaction;

public record GetByAccountIdTransactionQuery(Guid AccountId) : IQuery<List<Transaction>?>;