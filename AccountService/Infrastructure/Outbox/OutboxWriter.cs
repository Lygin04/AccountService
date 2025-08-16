using System.Text.Json;
using AccountService.Contracts;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Outbox.Enums;
using AccountService.Infrastructure.Outbox.Interfaces;

namespace AccountService.Infrastructure.Outbox;

public class OutboxWriter(IOutboxRepository outboxRepository) : IOutboxWriter
{
    public async Task WriteAsync<TPayload>(string routingKey, Envelope<TPayload> envelope, ITransaction? transaction = null)
    {
        var headers = new Dictionary<string, string>
        {
            ["X-Correlation-Id"] = envelope.Meta.CorrelationId.ToString(),
            ["X-Causation-Id"]   = envelope.Meta.CausationId.ToString()
        };

        var message = new OutboxMessage
        {
            Id = envelope.EventId,
            OccurredAt = envelope.OccurredAt,
            Type = typeof(TPayload).Name,
            RoutingKey = routingKey,
            PayloadJson = JsonSerializer.Serialize(envelope),
            HeadersJson = JsonSerializer.Serialize(headers),
            Status = OutboxStatus.Pending,
            Attempts = 0,
            NextAttemptAt = DateTime.UtcNow
        };
        
        await outboxRepository.AddAsync(message, transaction);
    }
}