using AccountService.Common.Enums;

namespace AccountService.Contracts;

public record MoneyCredited(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid OperationId
) : Event(EventId, OccurredAt);