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
) : Event(EventId, OccurredAt);