namespace AccountService.Contracts;

/// <summary>
/// Абстрактный класс события.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="OccurredAt">Дата создания события.</param>
public record Event(
    Guid EventId,
    DateTime OccurredAt)
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Meta Meta { get; init; } = new("v1", "account-service", Guid.NewGuid(), Guid.NewGuid());
}