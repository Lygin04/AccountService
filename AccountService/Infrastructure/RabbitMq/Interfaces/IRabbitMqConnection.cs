using RabbitMQ.Client;

namespace AccountService.Infrastructure.RabbitMq.Interfaces;

public interface IRabbitMqConnection
{
    IConnection? Connection { get; }
}