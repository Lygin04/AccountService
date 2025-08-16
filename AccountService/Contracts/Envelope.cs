namespace AccountService.Contracts;

public record Envelope<TPayload>(
    Guid EventId,
    DateTime OccurredAt,
    Meta Meta,
    TPayload Payload
);