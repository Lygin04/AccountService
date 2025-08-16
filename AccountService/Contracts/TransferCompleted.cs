using AccountService.Common.Enums;

namespace AccountService.Contracts;

public record TransferCompleted(
    Guid EventId,
    DateTime OccurredAt,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid TransferId
)
{
    public Meta Meta { get; init; } = new("1.0");
}