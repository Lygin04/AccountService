using System.Text;
using System.Text.Json;
using AccountService.Features.Accounts;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.RabbitMq.Interfaces;
using ExchangeType = RabbitMQ.Client.ExchangeType;

namespace AccountService.Infrastructure.Outbox;

// ReSharper disable once ClassNeverInstantiated.Global
public class InboxConsumer(
    IRabbitMqConnection connection,
    IInboxRepository inbox,
    IAccountRepository accounts,
    ILogger<InboxConsumer> logger)
{
    public async Task ConsumeAsync()
    {
        var task1 = AccountConsumeAsync("antifraud");
        var task2 = AccountConsumeAsync("audit");
        var task3 = AccountConsumeAsync("crm");
        var task4 = AccountConsumeAsync("notifications");

        await Task.WhenAll(task1, task2, task3, task4);
    }

    private async Task AccountConsumeAsync(string queueName)
    {
        var channel = connection.Connection!.CreateModel();
        channel.BasicQos(0, 1, false);
        channel.ExchangeDeclare("account.events", ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
        channel.QueueDeclare($"account.{queueName}", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind($"account.{queueName}", "account.events", "client.*", arguments: null);
        
        var ea = channel.BasicGet($"account.{queueName}", autoAck: false);
        if (ea == null)
            return;

        const string handlerName = nameof(InboxConsumer);
        var payload = Encoding.UTF8.GetString(ea.Body.ToArray());

        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            if (!root.TryGetProperty("Meta", out var meta) ||
                !meta.TryGetProperty("Version", out var versionProp) ||
                versionProp.GetString() is not "v1" ||
                !root.TryGetProperty("EventId", out var eventIdProp) ||
                !Guid.TryParse(eventIdProp.GetString(), out var messageId))
            {
                await inbox.AddDeadLetterAsync(Guid.NewGuid(), handlerName, payload, "Envelope or version is invalid");
                logger.LogWarning("INBOX DeadLetter: invalid envelope/version, rk={RoutingKey}", ea.RoutingKey);
                channel.BasicAck(ea.DeliveryTag, false);
                return;
            }

            if (await inbox.ExistsAsync(messageId, handlerName))
            {
                logger.LogInformation("INBOX Duplicate ignored: {MessageId}", messageId);
                channel.BasicAck(ea.DeliveryTag, false);
                return;
            }

            if (queueName == "antifraud")
            {
                switch (ea.RoutingKey)
                {
                    case "client.blocked":
                    {
                        var env = JsonSerializer.Deserialize<ClientBlockedPayload>(payload)!;
                        await HandleClientBlocked(env.ClientId);
                        break;
                    }
                    case "client.unblocked":
                    {
                        var env = JsonSerializer.Deserialize<ClientUnblockedPayload>(payload)!;
                        await HandleClientUnblocked(env.ClientId);
                        break;
                    }
                    default:
                        await inbox.AddDeadLetterAsync(messageId, handlerName, payload,
                            $"Unsupported rk {ea.RoutingKey}");
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                }
            }

            await inbox.AddConsumedAsync(messageId, handlerName);
            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "INBOX processing error, rk={RoutingKey}", ea.RoutingKey);
            channel.BasicNack(ea.DeliveryTag, false, requeue: false);
        }
    }
    
    private async Task HandleClientBlocked(Guid clientId)
    {
        await accounts.SetFrozenByOwnerAsync(clientId, true);
    }
    
    private async Task HandleClientUnblocked(Guid clientId)
    {
        await accounts.SetFrozenByOwnerAsync(clientId, false);
    }
    
    private record ClientBlockedPayload(Guid ClientId);
    private record ClientUnblockedPayload(Guid ClientId);
}