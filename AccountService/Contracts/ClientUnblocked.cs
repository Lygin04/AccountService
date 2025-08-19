namespace AccountService.Contracts;

/// <summary>
/// Событие разблокировки владельца.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
/// <param name="ClientId">Идентификатор владельца.</param>
public record ClientUnblocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid ClientId
) : Event(EventId, OccurredAt);