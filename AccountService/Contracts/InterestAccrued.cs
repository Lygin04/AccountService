namespace AccountService.Contracts;

/// <summary>
/// Событие начисления процентов.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="AccountId">Идентификатор счета.</param>
/// <param name="PeriodFrom">Начальная дата расчета процентов.</param>
/// <param name="PeriodTo">Конечная дата расчета процентов.</param>
/// <param name="Amount">Размер начисленных процентов.</param>
public record InterestAccrued(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    decimal? Amount
) : Event(EventId, OccurredAt);