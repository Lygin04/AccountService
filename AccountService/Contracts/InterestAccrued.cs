namespace AccountService.Contracts;

public record InterestAccrued(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    decimal? Amount
)
{
    public Meta Meta { get; init; } = new("v1", "account-service", Guid.NewGuid(), Guid.NewGuid());
}