using AccountService.Infrastructure.RabbitMq.Interfaces;
using RabbitMQ.Client;

namespace AccountService.Infrastructure.RabbitMq;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection? _connection;
    private readonly ConnectionFactory _factory;
    
    public RabbitMqConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"]!,
            Port = int.Parse(configuration["RabbitMQ:Port"]!),
            UserName = configuration["RabbitMQ:UserName"]!,
            Password = configuration["RabbitMQ:Password"]!,
            VirtualHost = "/",
            DispatchConsumersAsync = true
        };

        _connection = _factory.CreateConnection();
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public IConnection? Connection => _connection;

    public void Reconnect()
    {
        _connection = _factory.CreateConnection();
    }

#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
        _connection?.Dispose();
    }
}