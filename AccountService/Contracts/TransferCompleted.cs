using AccountService.Common.Enums;

namespace AccountService.Contracts;

/// <summary>
/// Событие перевода денег между счетами.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="SourceAccountId">Идентификатор счета из которого перевели деньги.</param>
/// <param name="DestinationAccountId">Идентификатор счета в который перевели деньги.</param>
/// <param name="Amount">Сумма перевода.</param>
/// <param name="Currency">Валюта.</param>
/// <param name="TransferId">Идентификатор транзакции.</param>
public record TransferCompleted(
    Guid EventId,
    DateTime OccurredAt,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    IsoCurrency Currency,
    Guid TransferId
) : Event(EventId, OccurredAt);