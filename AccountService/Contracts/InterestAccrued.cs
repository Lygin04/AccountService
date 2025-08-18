namespace AccountService.Contracts;

public record InterestAccrued(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    decimal? Amount
) : Event(EventId, OccurredAt);