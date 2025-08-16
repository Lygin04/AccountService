using AccountService.Common.Enums;

namespace AccountService.Contracts;

public record MoneyDebited(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid OperationId,
    string Reason
)
{
    public Meta Meta { get; init; } = new("1.0");
}