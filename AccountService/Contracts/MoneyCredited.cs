using AccountService.Common.Enums;

namespace AccountService.Contracts;

/// <summary>
/// Событие пополнения счета.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="AccountId">Идентификатор счета.</param>
/// <param name="Amount">Размер пополнения счета.</param>
/// <param name="Currency">Валюта.</param>
/// <param name="OperationId">Идентификатор транзакции.</param>
public record MoneyCredited(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid OperationId
) : Event(EventId, OccurredAt);