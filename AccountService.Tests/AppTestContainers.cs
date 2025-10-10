using AccountService.Infrastructure.Dapper;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Outbox;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.RabbitMq;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace AccountService.Tests;

public class AppTestContainers : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("bank")
        .WithUsername("postgres")
        .WithPassword("password")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(
            Wait.ForUnixContainer().UntilMessageIsLogged("database system is ready to accept connections"))
        .Build();


    private readonly RabbitMqContainer _rabbit = new RabbitMqBuilder()
        .WithImage("rabbitmq:management")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithPortBinding(5672, true)
        .WithPortBinding(15672, true)
        .WithBindMount(
            Path.GetFullPath("./rabbitmq/rabbitmq.conf"),
            "/etc/rabbitmq/rabbitmq.conf",
            AccessMode.ReadOnly)
        .WithBindMount(
            Path.GetFullPath("./rabbitmq/rabbitmq_definitions.json"),
            "/etc/rabbitmq/definitions.json",
            AccessMode.ReadOnly)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"))
        .Build();
    
    public string PostgresConnectionString {get; private set; } = string.Empty;
    public string RabbitHost { get; private set; } = string.Empty;
    public string RabbitPort { get; private set; } = string.Empty;
    
    public RabbitMqConnection RabbitConn = null!;
    public IOutboxRepository OutboxRepository = null!;
    public OutboxDispatcher OutboxDispatcher = null!;
    public ILogger<OutboxDispatcher> Logger = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbit.StartAsync();
        
        var pgHost = _postgres.Hostname;
        var pgPort = _postgres.GetMappedPublicPort(5432);
        
        PostgresConnectionString = $"Host={pgHost};Username=postgres;Password=password;Database=bank;Port={pgPort}";
        
        RabbitHost = _rabbit.Hostname;
        RabbitPort = _rabbit.GetMappedPublicPort(5672).ToString();
        
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BankDataBase:ConnectionString"] = PostgresConnectionString,
                ["BankDataBase:Provider"] = "PostgreSQL",
                ["RabbitMQ:HostName"] = RabbitHost,
                ["RabbitMQ:Port"] = RabbitPort,
                ["RabbitMQ:UserName"] = "guest",
                ["RabbitMQ:Password"] = "guest"
            })
            .Build();
        
        var settings = new BankDataBase(config);
        var dapperContext = new DapperContext<IDapperSettings>(settings);
        OutboxRepository = new OutboxRepository(dapperContext);

        RabbitConn = new RabbitMqConnection(config);
        Logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<OutboxDispatcher>();
        OutboxDispatcher = new OutboxDispatcher(OutboxRepository, RabbitConn, Logger);
    }

    public async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _rabbit.StopAsync();
    }
}