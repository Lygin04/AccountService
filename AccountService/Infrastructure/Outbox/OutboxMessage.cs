using AccountService.Infrastructure.Outbox.Enums;

namespace AccountService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; init; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime OccurredAt { get; init; }
    public string Type { get; init; } = string.Empty;
    public string RoutingKey { get; init; } = string.Empty;
    public string PayloadJson { get; init; } = string.Empty;
    public string HeadersJson { get; init; } = "{}";
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public int Attempts { get; init; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime NextAttemptAt { get; set; } = DateTime.UtcNow;
    // ReSharper disable once UnusedMember.Global
    public DateTime? PublishedAt { get; set; }
}