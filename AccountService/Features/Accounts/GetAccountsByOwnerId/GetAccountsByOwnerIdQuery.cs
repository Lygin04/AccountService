using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public record GetAccountsByOwnerIdQuery(Guid OwnerId) : IQuery<List<Account>>;