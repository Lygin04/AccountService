using System.Text;
using System.Text.Json;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.RabbitMq.Interfaces;
using RabbitMQ.Client;

namespace AccountService.Infrastructure.Outbox;

public class OutboxDispatcher(
    IOutboxRepository outboxRepository,
    IRabbitMqConnection rabbitMqConnection,
    ILogger<OutboxDispatcher> logger)
{
    private const int BatchSize = 100;
    
    public async Task DispatchBatch()
    {
        var due = await outboxRepository.TakeDueAsync(BatchSize);
        if (due.Count == 0) return;

        using var channel = rabbitMqConnection.Connection!.CreateModel();
        channel.ExchangeDeclare("account.events", ExchangeType.Topic, durable: true);

        foreach (var m in due)
        {
            try
            {
                var props = channel.CreateBasicProperties();
                props.ContentType = "application/json";

                // Заголовки
                var headers = JsonSerializer.Deserialize<Dictionary<string, object>>(m.HeadersJson) ?? new();
                props.Headers = headers.ToDictionary(k => k.Key, v => (object)Encoding.UTF8.GetBytes(v.Value.ToString()!));

                var body = Encoding.UTF8.GetBytes(m.PayloadJson);

                var t0 = DateTime.UtcNow;
                channel.BasicPublish("account.events", m.RoutingKey, props, body);
                var latency = (DateTime.UtcNow - t0).TotalMilliseconds;

                logger.LogInformation("OUTBOX Published: {EventId} {Type} rk={RoutingKey} attempts={Attempts} latencyMs={Latency}",
                    m.Id, m.Type, m.RoutingKey, m.Attempts, latency);

                await outboxRepository.MarkPublishedAsync(m.Id);
            }
            catch (Exception ex)
            {
                var next = CalcNextAttemptUtc(m.Attempts + 1);
                logger.LogWarning(ex, "OUTBOX Publish failed: {EventId} {Type} attempts={Attempts}. NextAttemptAt={Next}",
                    m.Id, m.Type, m.Attempts + 1, next);

                await outboxRepository.MarkFailedAsync(m.Id, next);
            }
        }
    }

    private static DateTime CalcNextAttemptUtc(int attempt)
    {
        // экспонента с джиттером: base 2^attempt, max ~ 5 мин
        var seconds = Math.Min(Math.Pow(2, attempt), 300);
        var jitter = Random.Shared.NextDouble() * 0.3 + 0.85; // +/-15%
        return DateTime.UtcNow.AddSeconds(seconds * jitter);
    }
}