using AccountService.Infrastructure.RabbitMq.Interfaces;
using RabbitMQ.Client;

namespace AccountService.Infrastructure.RabbitMq;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnection? _connection;

    public RabbitMqConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"]!,
            Port = int.Parse(configuration["RabbitMQ:Port"]!),
            UserName = configuration["RabbitMQ:UserName"]!,
            Password = configuration["RabbitMQ:Password"]!,
            VirtualHost = "/",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
    }

    public IConnection? Connection => _connection;

    public void Dispose()
    {
        _connection?.Dispose();
    }
}