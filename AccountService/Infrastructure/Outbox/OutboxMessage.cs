using AccountService.Infrastructure.Outbox.Enums;

namespace AccountService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; init; }
    public DateTime OccurredAt { get; init; }
    public string Type { get; init; } = string.Empty;
    public string RoutingKey { get; init; } = string.Empty;
    public string PayloadJson { get; init; } = string.Empty;
    public string HeadersJson { get; init; } = "{}";
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public int Attempts { get; set; }
    public DateTime NextAttemptAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
}