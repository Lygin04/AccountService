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

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public IConnection? Connection => _connection;

#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
        _connection?.Dispose();
    }
}