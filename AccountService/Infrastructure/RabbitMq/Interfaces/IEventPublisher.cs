namespace AccountService.Infrastructure.RabbitMq.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, string routingKey, Guid correlationId, Guid causationId);
}