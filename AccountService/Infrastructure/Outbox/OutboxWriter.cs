using System.Text.Json;
using AccountService.Contracts;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Outbox.Enums;
using AccountService.Infrastructure.Outbox.Interfaces;

namespace AccountService.Infrastructure.Outbox;

public class OutboxWriter(IOutboxRepository outboxRepository) : IOutboxWriter
{
    public async Task WriteAsync<TPayload>(string routingKey, TPayload payload, ITransaction? transaction = null)
        where TPayload : Event
    {
        var headers = new Dictionary<string, string>
        {
            ["X-Correlation-Id"] = payload.Meta.CorrelationId.ToString(),
            ["X-Causation-Id"]   = payload.Meta.CausationId.ToString()
        };

        var message = new OutboxMessage
        {
            Id = payload.EventId,
            OccurredAt = payload.OccurredAt,
            Type = typeof(TPayload).Name,
            RoutingKey = routingKey,
            PayloadJson = JsonSerializer.Serialize(payload),
            HeadersJson = JsonSerializer.Serialize(headers),
            Status = OutboxStatus.Pending,
            Attempts = 0,
            NextAttemptAt = DateTime.UtcNow
        };
        
        await outboxRepository.AddAsync(message, transaction);
    }
}