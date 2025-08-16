using System.Text;
using System.Text.Json;
using AccountService.Infrastructure.RabbitMq.Interfaces;
using RabbitMQ.Client;

namespace AccountService.Infrastructure.RabbitMq;

public class EventPublisher(IRabbitMqConnection connection) : IEventPublisher
{
    public Task PublishAsync<T>(T @event, string routingKey, Guid correlationId, Guid causationId)
    {
        using var channel = connection.Connection!.CreateModel();
        channel.ExchangeDeclare(exchange: "account.events", type: ExchangeType.Topic, durable: true);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var props = channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.MessageId = Guid.NewGuid().ToString();
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        props.Headers = new Dictionary<string, object>
        {
            ["X-Correlation-Id"] = correlationId.ToString(),
            ["X-Causation-Id"] = causationId.ToString()
        };

        channel.BasicPublish(
            exchange: "account.events",
            routingKey: routingKey,
            basicProperties: props,
            body: body
        );

        return Task.CompletedTask;
    }
}