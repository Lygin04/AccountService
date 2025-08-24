using AccountService.Common.Enums;

namespace AccountService.Contracts;

/// <summary>
/// Событие списание денег со счета.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="AccountId">Идентификатор счета.</param>
/// <param name="Amount">Размер списание счета.</param>
/// <param name="Currency">Валюта.</param>
/// <param name="OperationId">Идентификатор транзакции.</param>
public record MoneyDebited(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid OperationId,
    string Reason
) : Event(EventId, OccurredAt);