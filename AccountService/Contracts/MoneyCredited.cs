using AccountService.Common.Enums;

namespace AccountService.Contracts;

public record MoneyCredited(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid OperationId
)
{
    public Meta Meta { get; init; } = new("v1", "account-service", Guid.NewGuid(), Guid.NewGuid());
}