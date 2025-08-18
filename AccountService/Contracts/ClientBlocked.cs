namespace AccountService.Contracts;

public record ClientBlocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId
) : Event(EventId, OccurredAt);