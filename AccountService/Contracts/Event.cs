namespace AccountService.Contracts;

public record Event(
    Guid EventId,
    DateTime OccurredAt)
{
    public Meta Meta { get; init; } = new("v1", "account-service", Guid.NewGuid(), Guid.NewGuid());
}