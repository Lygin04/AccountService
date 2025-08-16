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
    public Meta Meta { get; init; } = new("1.0");
}