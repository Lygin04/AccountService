namespace AccountService.Contracts;

public record ClientUnblocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId
) : Event(EventId, OccurredAt);