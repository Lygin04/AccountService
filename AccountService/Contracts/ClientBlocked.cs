namespace AccountService.Contracts;

public record ClientBlocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid ClientId
)
{
    public Meta Meta { get; init; } = new("v1", "antifraud-service", Guid.NewGuid(), Guid.NewGuid());
}