using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;

namespace AccountService.Contracts;

public record AccountOpened(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    Guid OwnerId,
    IsoCurrency Currency,
    AccountType Type
)
{
    public Meta Meta { get; init; } = new("1.0");
}