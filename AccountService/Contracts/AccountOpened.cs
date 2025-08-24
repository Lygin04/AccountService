using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;

namespace AccountService.Contracts;

/// <summary>
/// Событие создание счета.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="AccountId">Идентификатор счета.</param>
/// <param name="OwnerId">Идентификатор владельца счета.</param>
/// <param name="Currency">Валюта.</param>
/// <param name="Type">Тип счета.</param>
public record AccountOpened(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    Guid OwnerId,
    IsoCurrency Currency,
    AccountType Type
) : Event(EventId, OccurredAt);